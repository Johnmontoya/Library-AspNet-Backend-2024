using Microsoft.AspNetCore.Identity;

namespace Backend.Core
{
    /// <summary>
    /// Permite validar los errores de validacion de la clase Identity
    /// </summary>
    public class CustomValidationIdentityError : IdentityErrorDescriber
    {
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
                Description = "Ya existe un usuasrio con ese email",
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
                Description = "El formato del email no es valido",
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
                Description = "Ya existe un usuario con ese nombre"
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
                Description = "El nombre de usuario no puede tener espacios ni carácteres especiales",
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
                Description = "La contraseña es demasiado corta",
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
                Description = "La contraseña debe tener al menos un carácter no alfanumerico"
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
                Description = "La contraseña debe tener al menos un número"
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
                Description = "La contraseña debe tener al menos una letra mayúscula"
            };
        }
    }
}
