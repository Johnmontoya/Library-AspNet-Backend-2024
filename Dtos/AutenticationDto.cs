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
}
