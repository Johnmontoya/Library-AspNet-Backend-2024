namespace Backend.Dtos
{
    /// <summary>
    /// Clase de Validacion de registro de usuario
    /// </summary>
    public class UsuarioDto
    {
        /// <summary>
        /// Identificador del registro
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Número de identificaion del usuario
        /// </summary>
        public int DNI { get; set; }

        /// <summary>
        /// Fecha de nacimiento del usuario
        /// </summary>
        public string FechaNacimiento { get; set; } = string.Empty;

        /// <summary>
        /// Direccion de residencia
        /// </summary>
        public string Direccion { get; set; } = string.Empty;

        /// <summary>
        /// Ciudad de residencia
        /// </summary>
        public string Ciudad { get; set;} = string.Empty;
    }
}
