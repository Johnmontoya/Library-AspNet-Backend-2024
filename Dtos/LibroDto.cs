namespace Backend.Dtos
{
    /// <summary>
    /// Registro de libro
    /// </summary>
    public class LibroDto
    {
        /// <summary>
        /// Id del registro
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Id de la categoria
        /// </summary>
        public string? CategoriaId { get; set; }

        /// <summary>
        /// Nombre del libro
        /// </summary>
        public string? Nombre { get; set; }

        /// <summary>
        /// Editorial del libro
        /// </summary>
        public string? Editorial { get; set; }

        /// <summary>
        /// Id del autor
        /// </summary>
        public string? AutorId { get; set; }
    }
}
