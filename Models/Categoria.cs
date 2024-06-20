using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    /// <summary>
    /// Permite registrar las categorias de los libros
    /// </summary>
    [Display(Name = "Categoria")]
    public class Categoria
    {
        /// <summary>
        /// Id primary key de la categoria
        /// </summary>
        /// <value>El id se incrementa automáticamente</value>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "Varchar(255)")]
        public string? Id { get; set; }

        /// <summary>
        /// Obtiene la clave de la categoria
        /// </summary>
        /// <value>Clave de la categoria de 1 a 9999</value>              
        public int Clave { get; set; }

        /// <summary>
        /// Obtiene el nombre de la categoria
        /// El column debe ser especificado para el motor de DB
        /// </summary>
        /// <value>El nombre de la categoria</value>
        [StringLength(80)]
        [Column(TypeName = "Varchar(80")]
        public string? Nombre { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Backend.Models.Categoria"/> class.
        /// </summary>
        public Categoria() => LibrosEscritos = new Collection<Libro>();

        /// <summary>
        /// Obtiene los libros escritos
        /// </summary>
        public virtual ICollection<Libro> LibrosEscritos { get; set; }
    }
}
