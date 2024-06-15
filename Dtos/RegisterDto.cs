using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    /// <summary>
    /// Clase de Validacion de registro de la tabla Aspnetusers
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Credencial de email
        /// </summary>
        [Required]
        public string? Email { get; set; }

        /// <summary>
        /// Credencial de username
        /// </summary>
        [Required]
        public string? UserName { get; set; }

        /// <summary>
        /// Credencial de contraseña
        /// </summary>
        [Required]
        public string? Password { get; set; }
    }
}
