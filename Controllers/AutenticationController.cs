using Backend.Core;
using Backend.DAO;
using Backend.Database;
using Backend.Dtos;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Controllers
{
    /// <summary>
    /// Controlador de la autenticacion y la tabla usuario
    /// </summary>
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class AutenticationController: ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Authentication> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Authentication> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private UsuarioDAO _usuarioDAO;
        private readonly IOTPService _otpService;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="roleManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="configuration"></param>
        /// <param name="mailService"></param>
        /// <param name="otpService"></param>
        public AutenticationController(AppDbContext context, UserManager<Authentication> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Authentication> signInManager, IConfiguration configuration, IMailService mailService, IOTPService otpService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mailService = mailService;
            _usuarioDAO = new UsuarioDAO(_context, _userManager);
            _otpService = otpService;
        }

        /// <summary>
        /// Iniciar Sesion
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
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

            if(user.LockoutEnabled == false)
            {
                return Unauthorized("La cuenta se encuentra inhabilitada");
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

        /// <summary>
        /// Registro de nuevo usuario
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
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

        /// <summary>
        /// Confirmar El email despues del registro de usuario
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Envio de email con un token para la recuperacion de contraseña
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Recuperacion de token y contraseña actual, y nueva para cambiar la contraseña
        /// </summary>
        /// <param name="resetPasswordDto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Datos del usuario autenticado y datos personales
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile")]
        public async Task<IActionResult> ProfileUser()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == null)
            {
                return BadRequest("Usuario no autenticado");
            }

            var user = await _userManager.FindByNameAsync(currentUserId!);

            if (user == null)
            {
                return BadRequest("Usuario no encontrado");
            }

            var usuario = await _usuarioDAO.GetUserId(user.Id);                      

            if (usuario == null || usuario.AuthenticationId != user.Id)
            {
                return BadRequest("Información del usuario no encontrada o inconsistente");
            }

            return Ok(new UserDetailDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = [.. await _userManager.GetRolesAsync(user)],
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                AccessFailedCount = user.AccessFailedCount,
                Usuario = new UsuarioDto
                {
                    Id = usuario.Id,
                    DNI = usuario.DNI,
                    FechaNacimiento = usuario.FechaNacimiento,
                    Direccion = usuario.Direccion,
                    Ciudad = usuario.Ciudad
                },
            });
        }

        /// <summary>
        /// Registro de datos personales del usuario autenticado
        /// </summary>
        /// <param name="usuarioDto"></param>
        /// <returns></returns>
        [HttpPost("usuario")]
        public async Task<IActionResult> RegisterDataUser([FromBody] UsuarioDto usuarioDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(currentUserId!);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }            

            var data = await _usuarioDAO.AgregarAsync(usuarioDto, user!);

            if (!data)
            {
                return BadRequest();
            }
            
            return Ok("Datos personales guardados");
        }

        /// <summary>
        /// Agregar el numero de celular y envio de OTP al correo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autenticationDto"></param>
        /// <returns></returns>
        [HttpPut("add-phonenumber/{id}")]
        public async Task<IActionResult> RegisterPhone([FromRoute] string id, [FromBody] AutenticationDto autenticationDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(currentUserId!);

            if (user == null)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var otp = await _otpService.GenerateAndSendOTP(autenticationDto.PhoneNumber!);

            user.PhoneNumber = autenticationDto.PhoneNumber;
            user.OTPSecurity = otp;
            var data = await _userManager.UpdateAsync(user);

            if (!data.Succeeded)
            {
                return BadRequest("Error al actualizar el número telefónico");
            }            

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"http://localhost:4200/verify-phone?user={user.Id}";

            HTMLMailDto htmlMailDto = new HTMLMailDto()
            {
                EmailToId = user.Email,
                EmailToName = otp,
                ResetLink = resetLink
            };

            _mailService.SendHTMLMail(htmlMailDto, "otpNumber");

            return Ok("OTP enviado.");
        }

        /// <summary>
        /// Verificar numero de celular y codigo OTP
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autenticationDto"></param>
        /// <returns></returns>
        [HttpPut("number-verify/{id}")]
        public async Task<IActionResult> VerifyOTP([FromRoute] string id, [FromBody] AutenticationDto autenticationDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(currentUserId!);

            if (user == null)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            if(user.OTPSecurity != autenticationDto.Otp || user.PhoneNumber != autenticationDto.PhoneNumber)
            {
                return BadRequest("OTP inválido.");
            }
                        
            user.PhoneNumberConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return BadRequest("Error al actualizar el estado de verificación del número telefónico.");
            }

            return Ok("OTP verificado con éxito.");            
        }

        /// <summary>
        /// Cambiar contraseña con usuario logueado
        /// </summary>
        /// <param name="id"></param>
        /// <param name="changePasswordDto"></param>
        /// <returns></returns>
        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword([FromRoute] string id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                return BadRequest("Usuario no encontrado");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword!, changePasswordDto.NewPassword!);

            if (!result.Succeeded)
            {
                return BadRequest("Error con el cambio de password");
            }

            return Ok("La contraseña ha sido cambiada");
        }

        /// <summary>
        /// Desactivar la cuenta
        /// </summary>
        /// <returns></returns>
        [HttpPut("deactived")]
        public async Task<IActionResult> DeactivedUser()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(currentUserId!);
            user!.LockoutEnabled = false;

            await _userManager.UpdateAsync(user);

            return Ok("Su cuenta ha sido inhabilitada");
        }

        /// <summary>
        /// Enviar email para reactivacion de la cuenta
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("active-email")]
        public async Task<IActionResult> ActiveUser(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email!);

            if(user is null)
            {
                return BadRequest("Usuario no encontrado");
            }

            var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "ActiveUser");
            var resetLink = $"http://localhost:4200/active-account?email={user.Email}&token={WebUtility.UrlEncode(token)}";

            //Implementar las funcionalidades del servicio de mensajeria
            HTMLMailDto htmlMailDto = new HTMLMailDto()
            {
                EmailToId = user.Email,
                EmailToName = user.UserName,
                ResetLink = resetLink
            };

            _mailService.SendHTMLMail(htmlMailDto, "activarCuenta");

            return Ok("Hemos enviado un email con un link para reactivar su cuenta");
        }

        /// <summary>
        /// Reactivacion de la cuenta meditante token y email
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("reactive-account")]
        public async Task<IActionResult> ReactivedAccount(string Email, string token)
        {
            var user = await _userManager.FindByEmailAsync(Email!);

            if(user is null)
            {
                return BadRequest("Usuario no encontrado");
            }

            var result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ActiveUser", token);

            if(!result)
            {
                return BadRequest("Error con el token de validacion");
            }

            user.LockoutEnabled = true;
            await _userManager.UpdateAsync(user);

            return Ok("La cuenta ha sido activada");
        }

        /// <summary>
        /// Modificar los datos personales del usuario
        /// </summary>
        /// <param name="usuarioDto"></param>
        /// <returns></returns>
        [HttpPut("usuario")]
        public async Task<IActionResult> ModificarUsuario([FromBody] UsuarioDto usuarioDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var auth = await _userManager.FindByNameAsync(currentUserId!);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _usuarioDAO.ObtenerPorIdAsync(id: usuarioDto.Id!);
            if (user is null)
            {
                return BadRequest("Usuario no encontrado");
            }

            var result = await _usuarioDAO.ModificarAsync(usuarioDto.Id!, usuarioDto, auth!);

            if (!result)
            {
                return BadRequest("Error al actualizar los datos");
            }

            return Ok("Datos personales actualizados");
        }

        private string GenerateJwtToken(Authentication user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;

            // Crear una lista de claims y agregar los claims básicos
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Agregar cada rol como un claim
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

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
