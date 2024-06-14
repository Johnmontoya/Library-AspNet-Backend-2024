using Backend.Database;
using Backend.Dtos;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.UriParser;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers
{
    public class AutenticationController: ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Authentication> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Authentication> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;

        public AutenticationController(AppDbContext context, UserManager<Authentication> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Authentication> signInManager, IConfiguration configuration, IMailService mailService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mailService = mailService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email!);

            if (user == null) 
            {
                return Unauthorized("Usuario o password invalido");
            }

            if (!user.EmailConfirmed) 
            {
                return Unauthorized("El email no ha sido confirmado");
            }

            var lockUser = await _userManager.IsLockedOutAsync(user);

            if (lockUser)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                return Unauthorized($"El usuario ha sido bloqueado hasta {lockoutEnd?.ToString("yyyy-MM-dd HH:mm:ss")}");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password!, isPersistent: false, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                return Unauthorized($"El usuario ha sido bloqueado hasta {lockoutEnd?.ToString("yyyy-MM-dd HH:mm:ss")}");
            }
            else if (!result.Succeeded)
            {
                await _userManager.AccessFailedAsync(user);
                return Unauthorized("Usuario o password invalido");
            }
            else
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }

            var user = new Authentication
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password!);

            if (!result.Succeeded) 
            {
                return BadRequest();
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var resetLink = $"http://localhost:4200/confirm-email?userId={user.Id}&token={WebUtility.UrlEncode(token)}";

            HTMLMailDto hTMLMailDto = new HTMLMailDto()
            {
                EmailToId = user.Email,
                EmailToName = user.UserName,
                ResetLink = resetLink
            };

            _mailService.SendHTMLMail(hTMLMailDto, "EmailConfirmed");

            await _userManager.AddToRoleAsync(user, "User");

            return Ok();
        }

        [HttpGet("Confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest("Invalid confirmation request.");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest("Usuario no encontrado");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if(!result.Succeeded)
            {
                return BadRequest("Error al confirmar");                
            }

            return Ok("Email confirmado"); 
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string Email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(Email);

                if (user == null)
                {
                    return BadRequest("El usuario no se encuentra");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"http://localhost:4200/reset-password?email={user.Email}&token={WebUtility.UrlEncode(token)}";

                //Implementar las funcionalidades del servicio de mensajeria
                HTMLMailDto htmlMailDto = new HTMLMailDto()
                {
                    EmailToId = user.Email,
                    EmailToName = user.UserName,
                    ResetLink = resetLink
                };

                _mailService.SendHTMLMail(htmlMailDto, "resetPassword");

                return Ok("Hemos enviado un email con un link para proceder con el cambio de contraseña");
            }
            catch (Exception err)
            {
                Console.WriteLine(err);
                return BadRequest("Ha ocurrido un error con el proveedor del servicio de correo");
            }
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email!);

            if (user == null)
            {
                return BadRequest("El usuario no se encuentra");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token!, resetPasswordDto.NewPassword!);

            if(!result.Succeeded)
            {
                return BadRequest("Error al cambiar el password");
            }

            return Ok("El password ha sido cambiado");
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSetting:securityKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: _configuration["JWTSetting:ValidIssuer"],
            audience: _configuration["JWTSetting:ValidAudience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
