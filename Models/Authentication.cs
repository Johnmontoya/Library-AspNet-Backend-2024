using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    /// <summary>
    /// Clase de autenticacion
    /// </summary>
    [Display(Name = "Authentication")]
    public class Authentication: IdentityUser
    {
        /// <summary>
        /// Datos de la tabla usuario
        /// </summary>
        public Usuario? Usuario { get; set; }

        /// <summary>
        /// Registro de codigo OTP para número de celular
        /// </summary>
        public string? OTPSecurity { get; set; }
    }
}
