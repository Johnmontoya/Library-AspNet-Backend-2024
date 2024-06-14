using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.CodeDom.Compiler;
using System.IdentityModel.Tokens.Jwt;
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

        public AutenticationController(AppDbContext context, UserManager<Authentication> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Authentication> signInManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
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

            await _userManager.AddToRoleAsync(user, "User");

            return Ok();
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
