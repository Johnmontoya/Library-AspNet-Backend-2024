namespace Backend.Dtos
{
    /// <summary>
    /// Clase de Validacion de Autenticacion
    /// </summary>
    public class AutenticationDto
    {
        /// <summary>
        /// Id del registro
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Número de celular
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Codigo OTP
        /// </summary>
        public string? Otp { get; set; }
    }

    /// <summary>
    /// Respuesta para el Mapper de Usuario
    /// </summary>
    public class AutenticationDtoResponse
    {
        /// <summary>
        /// Nombre de usuario 
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Email del usuario
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Número de celular
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Datos de la tabla usuario
        /// </summary>
        public UsuarioDto? Usuario { get; set; }
    }
}
