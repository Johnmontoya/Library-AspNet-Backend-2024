using Backend.Core.Errors;
using Backend.Core.OTP;
using Backend.DAO;
using Backend.Database;
using Backend.Dtos;
using Backend.Interfaces;
using Backend.Models;
using Backend.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
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
        private IValidator<UsuarioDto> _validator;
        private readonly LocService _locService;

        /// <summary>
        /// Mensaje de error personalizado
        /// </summary>
        public CustomError? customError;

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
        /// <param name="validator"></param>
        /// <param name="locService"></param>
        public AutenticationController(
            AppDbContext context, 
            UserManager<Authentication> userManager, 
            RoleManager<IdentityRole> roleManager, 
            SignInManager<Authentication> signInManager, 
            IConfiguration configuration, 
            IMailService mailService, 
            IOTPService otpService,
            IValidator<UsuarioDto> validator,
            LocService locService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mailService = mailService;
            _locService = locService;
            _otpService = otpService;
            _validator = validator;
            _usuarioDAO = new UsuarioDAO(_context, _userManager, _locService);
        }

        /// <summary>
        /// Iniciar Sesion
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email!);

            if (user == null) 
            {
                return Unauthorized(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("UserPassInvalid")),
                    status = 401,
                    errors = null
                });
            }

            if (!user.EmailConfirmed) 
            {
                return Unauthorized(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("EmailNotConfirm"), user.Email),
                    status = 401,
                    errors = null
                });
            }

            if(user.LockoutEnabled == false)
            {
                return Unauthorized(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("AccountDeactivated"), user.Email),
                    status = 401,
                    errors = null
                });
            }

            var lockUser = await _userManager.IsLockedOutAsync(user);

            if (lockUser)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                return Unauthorized(new ApiResponseDto
                {                    
                    title = $"{String.Format(_locService.GetLocalizedString("AccountBlocked"), user.UserName)} {lockoutEnd?.ToString("yyyy-MM-dd HH:mm:ss")}",
                    status = 401,
                    errors = null
                });
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password!, isPersistent: false, lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                return Unauthorized(new ApiResponseDto
                {
                    title = $"{String.Format(_locService.GetLocalizedString("AccountBlocked"), user.UserName)} {lockoutEnd?.ToString("yyyy-MM-dd HH:mm:ss")}",
                    status = 401,
                    errors = null
                });
            }
            else if (!result.Succeeded)
            {
                await _userManager.AccessFailedAsync(user);
                return Unauthorized(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("UserPassInvalid")),
                    status = 401,
                    errors = null
                });
            }
            else
            {
                await _userManager.ResetAccessFailedCountAsync(user);
                var token = GenerateJwtToken(user);
                return Ok(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("Welcome"), user.UserName),
                    status = 200,
                    token = token,
                    errors = null
                });
            }
        }

        /// <summary>
        /// Registro de nuevo usuario
        /// </summary>
        /// <param name="registerDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<ApiResponseDto>> Register([FromBody] RegisterDto registerDto)
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
                var errorsAuth = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.Description).ToList()
                    );

                return BadRequest(new ApiResponseDto
                {
                    status = 400,
                    title = String.Format(_locService.GetLocalizedString("ErrorRequest")),
                    errors = errorsAuth
                });
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

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("LinkSendEmail")),
                status = 201,
                errors = null
            });
        }

        /// <summary>
        /// Confirmar El email despues del registro de usuario
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Confirm-email")]
        public async Task<ActionResult<ApiResponseDto>> ConfirmEmail([FromQuery] string userId, string token)
        {
            if (userId == null || token == null)
            {
                return BadRequest(new ApiResponseDto
                    {
                        title = String.Format(_locService.GetLocalizedString("UserTokenInvalid")),
                        status = 404,
                        errors = null
                    });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFound")),
                    status = 404,
                    errors = null
                });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if(!result.Succeeded)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorRequest")),
                    status = 400,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("EmailConfirm")),
                status = 200,
                errors = null
            }); 
        }

        /// <summary>
        /// Envio de email con un token para la recuperacion de contraseña
        /// </summary>
        /// <param name="resetPasswordDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponseDto>> ForgotPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email!);

                if (user == null)
                {
                    return NotFound(new ApiResponseDto
                    {
                        title = String.Format(_locService.GetLocalizedString("NotFound")),
                        status = 404,
                        errors = null
                    });
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

                return Ok(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("LinkSendPassword")),
                    status = 200,
                    errors = null
                });
            }
            catch (Exception err)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorServerEmail")),
                    status = 500,
                    errors = new Dictionary<string, List<string>>
                    {
                        { "Categoria", new List<string> { err.Message } }
                    }
                });
            }
        }

        /// <summary>
        /// Recuperacion de token y contraseña actual, y nueva para cambiar la contraseña
        /// </summary>
        /// <param name="resetPasswordDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponseDto>> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email!);

            if (user == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFound")),
                    status = 404,
                    errors = null
                });
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token!, resetPasswordDto.NewPassword!);

            if(!result.Succeeded)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorPassword")),
                    status = 500,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("PasswordSuccess")),
                status = 200,
                errors = null
            });
        }

        /// <summary>
        /// Datos del usuario autenticado y datos personales
        /// </summary>
        /// <returns></returns>
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponseDto>> ProfileUser()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("Unauthorized")),
                    status = 401,
                    errors = null
                });
            }

            var user = await _userManager.FindByNameAsync(currentUserId!);

            if (user == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFound")),
                    status = 404,
                    errors = null
                });
            }

            var usuario = await _usuarioDAO.GetUserId(user.Id);                      

            if (usuario == null || usuario.AuthenticationId != user.Id)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ProfileInfo")),
                    status = 400,
                    errors = null
                });
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
        public async Task<ActionResult<ApiResponseDto>> RegisterDataUser([FromBody] UsuarioDto usuarioDto)
        {
            var result = await _validator.ValidateAsync(usuarioDto);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(currentUserId!);

            if (!result.IsValid)
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(modelState);
            }

            bool exists = _context.Usuarios.Any(u => u.AuthenticationId == user!.Id);

            if (exists) 
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ProfileAlready")),
                    status = 400,
                    errors = null
                });
            }

            var data = await _usuarioDAO.AgregarAsync(usuarioDto, user!);

            if (!data)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorRequest")),
                    status = 400,
                    errors = null
                });
            }
            
            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("ProfileSaved"), "guardados"),
                status = 201,
                errors = null
            });
        }

        /// <summary>
        /// Modificar los datos personales del usuario
        /// </summary>
        /// <param name="usuarioDto"></param>
        /// <returns></returns>
        [HttpPut("usuario")]
        public async Task<ActionResult<ApiResponseDto>> ModificarUsuario([FromBody] UsuarioDto usuarioDto)
        {
            var result = await _validator.ValidateAsync(usuarioDto);
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var auth = await _userManager.FindByNameAsync(currentUserId!);

            if (!result.IsValid)
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(modelState);
            }

            var user = await _usuarioDAO.ObtenerPorIdAsync(id: usuarioDto.Id!);
            if (user is null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFound")),
                    status = 404,
                    errors = null
                });
            }

            var data = await _usuarioDAO.ModificarAsync(usuarioDto.Id!, usuarioDto, auth!);

            if (!data)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorRequest")),
                    status = 400,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("ProfileSaved"), "actualizados"),
                status = 200,
                errors = null
            });
        }

        /// <summary>
        /// Agregar el numero de celular y envio de OTP al correo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autenticationDto"></param>
        /// <returns></returns>
        [HttpPut("add-phonenumber/{id}")]
        public async Task<ActionResult<ApiResponseDto>> RegisterPhone([FromRoute] string id, [FromBody] AutenticationDto autenticationDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(currentUserId!);

            if (user == null)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFound")),
                    status = 404,
                    errors = null
                });
            }

            var otp = await _otpService.GenerateAndSendOTP(autenticationDto.PhoneNumber!);

            user.PhoneNumber = autenticationDto.PhoneNumber;
            user.OTPSecurity = otp;
            var data = await _userManager.UpdateAsync(user);

            if (!data.Succeeded)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorRequest")),
                    status = 404,
                    errors = null
                });
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

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("LinkSendOTP")),
                status = 200,
                errors = null
            });
        }

        /// <summary>
        /// Verificar numero de celular y codigo OTP
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autenticationDto"></param>
        /// <returns></returns>
        [HttpPut("number-verify/{id}")]
        public async Task<ActionResult<ApiResponseDto>> VerifyOTP([FromRoute] string id, [FromBody] AutenticationDto autenticationDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(currentUserId!);

            if (user == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFound")),
                    status = 404,
                    errors = null
                });
            }

            if(user.OTPSecurity != autenticationDto.Otp || user.PhoneNumber != autenticationDto.PhoneNumber)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("OTPInvalid")),
                    status = 400,
                    errors = null
                });
            }
                        
            user.PhoneNumberConfirmed = true;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorRequest")),
                    status = 400,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("OTPSuccess")),
                status = 200,
                errors = null
            });            
        }

        /// <summary>
        /// Cambiar contraseña con usuario logueado
        /// </summary>
        /// <param name="id"></param>
        /// <param name="changePasswordDto"></param>
        /// <returns></returns>
        [HttpPut("change-password/{id}")]
        public async Task<ActionResult<ApiResponseDto>> ChangePassword([FromRoute] string id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFound")),
                    status = 404,
                    errors = null
                });
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword!, changePasswordDto.NewPassword!);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorRequest")),
                    status = 400,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("PasswordSuccess")),
                status = 200,
                errors = null
            });
        }

        /// <summary>
        /// Desactivar la cuenta
        /// </summary>
        /// <returns></returns>
        [HttpPut("deactived")]
        public async Task<ActionResult<ApiResponseDto>> DeactivedUser()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByNameAsync(currentUserId!);
            user!.LockoutEnabled = false;

            await _userManager.UpdateAsync(user);

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("AccountDeactivated"), user.UserName),
                status = 200,
                errors = null
            });
        }

        /// <summary>
        /// Enviar email para reactivacion de la cuenta
        /// </summary>
        /// <param name="resetPasswordDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("active-email")]
        public async Task<IActionResult> ActiveUser([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email!);

            if(user is null)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFound")),
                    status = 400,
                    errors = null
                });
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

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("LinkSendAccount")),
                status = 200,
                errors = null
            });
        }

        /// <summary>
        /// Reactivacion de la cuenta meditante token y email
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut("reactive-account")]
        public async Task<ActionResult<ApiResponseDto>> ReactivedAccount([FromQuery] string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email!);

            if(user is null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFound")),
                    status = 404,
                    errors = null
                });
            }

            var result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ActiveUser", token);

            if(!result)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorRequest")),
                    status = 400,
                    errors = null
                });
            }

            user.LockoutEnabled = true;
            await _userManager.UpdateAsync(user);

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("AccountActivated"), user.UserName),
                status = 200,
                errors = null
            });
        }
                
        private string GenerateJwtToken(Authentication user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;

            // Crear una lista de claims y agregar los claims básicos
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
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
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
