namespace Backend.Dtos
{
    public class UsuarioDto
    {
        /// <summary>
        /// Identificador del registro
        /// </summary>
        public string? Id { get; set; }
        public int DNI { get; set; }
        public string FechaNacimiento { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set;} = string.Empty;
    }
}
