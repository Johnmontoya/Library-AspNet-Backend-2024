using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    /// <summary>
    /// Clase de usuario
    /// </summary>
    [Display(Name = "Usuario")]
    public class Usuario
    {
        /// <summary>
        /// Id de la tabla usuario
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "Varchar(255)")]
        public string? Id { get; set; }

        /// <summary>
        /// Número de identificación del usuario
        /// </summary>
        public int DNI { get; set; }

        /// <summary>
        /// Fecha de nacimiento
        /// </summary>
        public string FechaNacimiento { get; set; } = string.Empty;

        /// <summary>
        /// Direccion de residencia
        /// </summary>
        public string Direccion { get; set; } = string.Empty;

        /// <summary>
        /// Ciudad de residencia
        /// </summary>
        public string Ciudad { get; set; } = string.Empty;

        /// <summary>
        /// Id del usuario logueado
        /// </summary>
        [ForeignKey("Authentication")]
        public string? AuthenticationId { get; set; }

        /// <summary>
        /// Datos del usuario logueado
        /// </summary>
        public Authentication? Authentication { get; set; }

    }
}
