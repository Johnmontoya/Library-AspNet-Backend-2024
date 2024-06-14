namespace Backend.Dtos
{
    /// <summary>
    /// Clase para configuracion del servicio de Email
    /// </summary>
    public class MailSettingDto
    {
        /// <summary>
        /// Servidor
        /// </summary>
        public string? Server { get; set; }

        /// <summary>
        /// Puerto en el que funciona
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Nombre de quien envia el email
        /// </summary>
        public string? SenderName { get; set; }

        /// <summary>
        /// Email de quien envia el correo
        /// </summary>
        public string? SenderEmail { get; set; }

        /// <summary>
        /// Cuenta del sistema de mensajeria
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Contraseña del sistema de mensajeria
        /// </summary>
        public string? Password { get; set; }
    }
}
