using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Dtos
{
    /// <summary>
    /// Registro autor
    /// </summary>
    public class AutorDto
    {
        /// <summary>
        /// Identificador del registro
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Nombre del autor
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Nacionalidad del autor
        /// </summary>
        public string? Nacionalidad { get; set; }
    }
}
