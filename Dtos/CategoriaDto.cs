namespace Backend.Dtos
{
    /// <summary>
    /// Registro categoria
    /// </summary>
    public class CategoriaDto
    {
        /// <summary>
        /// Identificador del registro
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Clave unica
        /// </summary>
        public int Clave { get; set; }

        /// <summary>
        /// Nombre de la categoria
        /// </summary>
        public string? Nombre { get; set; }
    }
}
