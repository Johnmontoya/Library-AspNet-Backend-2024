using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Permite registrar los libros
    /// </summary>
    [Display(Name = "Libro")]
    public class Libro
    {
        /// <summary>
        /// Id primary key del producto
        /// </summary>
        /// <value>El id se incrementa automáticamente</value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "Varchar(255)")]
        public string? Id { get; set; }

        /// <summary>
        /// Id de la tabla categoria
        /// </summary>
        [Column(TypeName = "Varchar(255)")]
        public string? CategoriaId { get; set; }

        /// <summary>
        /// Datos de la categoria
        /// </summary>
        public Categoria? Categoria { get; set; }

        /// <summary>
        /// Obtiene el nombre del producto
        /// El column debe ser especificado para el motor de DB
        /// </summary>
        /// <value>El nombre del producto</value>
        [Column(TypeName = "Varchar(80)")]
        public string? Nombre { get; set; }

        /// <summary>
        /// Editorial en cargada de la distribucion
        /// </summary>
        [Column(TypeName = "Varchar(80)")]
        public string? Editorial { get; set; }

        /// <summary>
        /// id del autor
        /// </summary>
        [Column(TypeName = "Varchar(255)")]
        public string? AutorId { get; set; }

        /// <summary>
        /// Datos del autor
        /// </summary>
        public Autor? Autor { get; set; }
    }
}
