namespace Backend.Dtos
{
    /// <summary>
    /// Registro necesario para enviar en el correo
    /// </summary>
    public class HTMLMailDto
    {
        /// <summary>
        /// Id Email del usuario
        /// </summary>
        public string? EmailToId { get; set; }

        /// <summary>
        /// Nombre del usuario a quien se envia
        /// </summary>
        public string? EmailToName { get; set; }

        /// <summary>
        /// Link de cambio de contraseña con token incluido
        /// </summary>
        public string? ResetLink { get; set; }
    }
}
