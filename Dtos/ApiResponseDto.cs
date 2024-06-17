using Microsoft.AspNetCore.Identity;

namespace Backend.Dtos
{
    /// <summary>
    /// Clase para el manejo de errores o mensajes
    /// </summary>
    public class ApiResponseDto
    {
        /// <summary>
        /// Codigo de estado
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// Mensaje para interfaz
        /// </summary>
        public string? title { get; set; }

        /// <summary>
        /// Token de usuario
        /// </summary>
        public string? token { get; set; } = string.Empty;

        /// <summary>
        /// Lista de errores
        /// </summary>
        public Dictionary<string, List<string>>? errors { get; set; }
    }
}
