using Backend.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Backend.Core.Errors
{
    /// <summary>
    /// Permite validar los errores de validacion de la clase Identity
    /// </summary>
    public class CustomValidationIdentityError : IdentityErrorDescriber
    {
        private readonly IStringLocalizer _localizedizer;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="factory"></param>
        public CustomValidationIdentityError(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName!);
            _localizedizer = factory.Create("SharedResource", assemblyName.Name!);
        }

        /// <summary>
        /// El email de usuario ya existe
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = _localizedizer["DuplicateEmail", email],
            };
        }

        /// <summary>
        /// El email debe ser valido
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public override IdentityError InvalidEmail(string? email)
        {
            return new IdentityError
            {
                Code = nameof(InvalidEmail),
                Description = _localizedizer["InvalidEmail", email!],
            };
        }

        /// <summary>
        /// El nombre de usuario ya existe
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = _localizedizer["DuplicateUserName", userName]
            };
        }

        /// <summary>
        /// Valida el nombre del usuario
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override IdentityError InvalidUserName(string? userName)
        {
            return new IdentityError
            {
                Code = nameof(InvalidUserName),
                Description = _localizedizer["InvalidUserName"],
            };
        }

        /// <summary>
        /// La contraseña no debe ser inferior a 6 caractares
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = _localizedizer["PasswordTooShort"],
            };
        }

        /// <summary>
        /// Requiere que el password tenga tanto letras como números
        /// </summary>
        /// <returns></returns>
        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = _localizedizer["PasswordRequiresNonAlphanumeric"]
            };
        }

        /// <summary>
        /// Las contraseñas necesitan de un valor numerico
        /// </summary>
        /// <returns></returns>
        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresDigit),
                Description = _localizedizer["PasswordRequiresDigit"]
            };
        }

        /// <summary>
        /// Las contraseña deben contener al menos una letra mayúscula
        /// </summary>
        /// <returns></returns>
        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError
            {
                Code = nameof(PasswordRequiresUpper),
                Description = _localizedizer["PasswordRequiresUpper"]
            };
        }
    }
}
