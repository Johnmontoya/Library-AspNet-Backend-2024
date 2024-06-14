namespace Backend.Dtos
{
    /// <summary>
    /// Registro para el cambio de contraseña
    /// </summary>
    public class ResetPasswordDto
    {
        /// <summary>
        /// Email al cual se le va a cambiar la contraseña
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Token que se recibe como parametro
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// NUeva contraseña para cambiar
        /// </summary>
        public string? NewPassword { get; set; }
    }
}
