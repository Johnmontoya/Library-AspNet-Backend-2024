using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Guarda los autores
    /// </summary>
    [Display(Name = "Autor")]
    public class Autor
    {
        /// <summary>
        /// Id del Autor
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "Varchar(255)")]
        public string? Id { get; set; }

        /// <summary>
        /// Nombre del autor
        /// </summary>
        [Column(TypeName = "VARCHAR(80)")]
        public string? Nombre { get; set; }

        /// <summary>
        /// Nacionalidad del autor
        /// </summary>
        [Column(TypeName = "VARCHAR(80)")]
        public string? Nacionalidad { get; set; }
    }
}
