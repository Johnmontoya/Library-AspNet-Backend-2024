namespace Backend.Dtos
{
    /// <summary>
    /// Registro para el cambio de contraseña desde usuario logueado
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// Contraseña actual
        /// </summary>
        public string? CurrentPassword { get; set; }

        /// <summary>
        /// Nueva contraseña
        /// </summary>
        public string? NewPassword { get; set; }
    }
}
