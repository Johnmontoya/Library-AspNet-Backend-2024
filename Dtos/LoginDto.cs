using System.ComponentModel.DataAnnotations;

namespace Backend.Dtos
{
    /// <summary>
    /// Clase de Validacion de Autenticacion
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Credencial de email
        /// </summary>
        [Required]
        public string? Email { get; set; }

        /// <summary>
        /// Credencial de contraseña
        /// </summary>
        [Required]
        public string? Password { get; set; }
    }
}
