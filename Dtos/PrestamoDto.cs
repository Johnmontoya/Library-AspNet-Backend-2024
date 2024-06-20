using Backend.Models;

namespace Backend.Dtos
{
    /// <summary>
    /// Registro del prestamo
    /// </summary>
    public class PrestamoDto
    {
        /// <summary>
        /// Id del registro
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Id del usuario
        /// </summary>
        public string? AutenticacionId { get; set; }

        /// <summary>
        /// Id del libro
        /// </summary>
        public string? LibroId { get; set; }

        /// <summary>
        /// Fecha de inicio del prestamos del libro
        /// </summary>
        public string? FechaPrestamo { get; set; }

        /// <summary>
        /// Fecha de finalizacion del prestamo y devolucion del libro
        /// </summary>
        public string? FechaDevolucion { get; set; }
    }

    /// <summary>
    /// Respuesta para el Mapper de Prestamo
    /// </summary>
    public class PrestamoDtoResponse
    {
        /// <summary>
        /// Id del registro
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Datos del usuario
        /// </summary>
        public AutenticationDtoResponse? Autenticacion { get; set; }

        /// <summary>
        /// Datos del libro rentado
        /// </summary>
        public LibroDtoResponse? Libro { get; set; }

        /// <summary>
        /// Fecha de inicio del prestamos del libro
        /// </summary>
        public string? FechaPrestamo { get; set; }

        /// <summary>
        /// Fecha de finalizacion del prestamo y devolucion del libro
        /// </summary>
        public string? FechaDevolucion { get; set; }
    }
}
