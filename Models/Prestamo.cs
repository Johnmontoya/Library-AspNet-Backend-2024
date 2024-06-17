using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Guarda los prestamos de los libros a los usuarios
    /// </summary>
    [Display(Name = "Prestamo")]
    public class Prestamo
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "Varchar(255)")]
        public string? Id { get; set; }

        /// <summary>
        /// Id del usuario
        /// </summary>
        [Column(TypeName = "Varchar(255)")]
        public string? AutenticacionId { get; set; }

        /// <summary>
        /// Datos del usuario
        /// </summary>
        public Authentication? Autenticacion { get; set; }

        /// <summary>
        /// Id del libro
        /// </summary>
        [Column(TypeName = "Varchar(255)")]
        public string? LibroId { get; set; }

        /// <summary>
        /// Datos del libro
        /// </summary>
        public Libro? Libro { get; set; }

        /// <summary>
        /// Fecha de inicio del prestamos del libro
        /// </summary>
        public string? FechaPrestamo { get; set; }

        /// <summary>
        /// Fecha de finalizacion del prestamo y devolucion del libro
        /// </summary>
        public string? FechaDevolucion { get; set; }
    }
}
