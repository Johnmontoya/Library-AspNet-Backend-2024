using Backend.Models;
using System.Text.Json.Serialization;

namespace Backend.Dtos
{
    /// <summary>
    /// Base para la informacion del usuario logueado
    /// </summary>
    public class UserDetailDto
    {
        /// <summary>
        /// Id del usuario
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Nombre de usuario 
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Email del usuario
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Rol del usuario
        /// </summary>
        public string[]? Roles { get; set; }

        /// <summary>
        /// Número de celular
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// El número de celular confirmado
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Numero de veces que ha fallado al intentar iniciar sesion
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// Datos de la tabla usuario
        /// </summary>
        public UsuarioDto? Usuario { get; set; }
    }
}
