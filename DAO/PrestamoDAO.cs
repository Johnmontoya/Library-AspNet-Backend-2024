using Backend.Core;
using Backend.Core.Errors;
using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Backend.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;

namespace Backend.DAO
{
    /// <summary>
    /// Funciones para acceso a los datos para los prestamos de libros
    /// </summary>
    public class PrestamoDAO
    {
        private readonly AppDbContext _context;
        private AccessDAO<Prestamo> prestamoDAO;
        private readonly LocService _locService;

        /// <summary>
        /// Mensaje de error personalizado
        /// </summary>
        public CustomError? customError;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="locService"></param>
        public PrestamoDAO(AppDbContext context, LocService locService)
        {
            _context = context;
            _locService = locService;
            prestamoDAO = new AccessDAO<Prestamo>(context, _locService);
        }

        /// <summary>
        /// Obtiene todos los prestamos
        /// </summary>
        /// <returns></returns>
        public async Task<List<Prestamo>> ObtenerTodoAsync()
        {
            return _context.Prestamos.ToList();
        }

        /// <summary>
        /// Obtiene un prestamo por su Id
        /// </summary>
        /// <param name="id">Id del prestamo</param>
        /// <returns></returns>
        public async Task<Prestamo> ObtenerPorIdAsync(string id)
        {
            var result = _context.Prestamos.FirstOrDefaultAsync(c => c.Id == id);
            return result.Result!;

        }

        /// <summary>
        /// Permite registrar un nuevo prestamo
        /// </summary>
        public async Task<bool> AgregarAsync(PrestamoDto prestamoDto, Authentication auth , Libro libro)
        {
            DateTime fechaToday = DateTime.Now;
            var prestamo = new Prestamo
            {
                AutenticacionId = auth.Id,
                Autenticacion = auth,
                LibroId = prestamoDto.LibroId,
                Libro = libro,
                FechaPrestamo = prestamoDto.FechaPrestamo != null ? prestamoDto.FechaPrestamo : fechaToday.ToString("yyyy-MM-dd"),
                FechaDevolucion = prestamoDto.FechaDevolucion
            };

            List<IRegla> reglas = new List<IRegla>();

            var result = await prestamoDAO.AgregarAsync(prestamo, reglas);

            if (result)
            {
                return true;
            }
            else
            {
                customError = prestamoDAO.customError;
                return false;
            }
        }

        /// <summary>
        /// Permite actualizar un nuevo prestamo
        /// </summary>
        public async Task<bool> ModificarAsync(string id, PrestamoDto prestamoDto, Authentication auth, Libro libro)
        {
            DateTime fechaToday = DateTime.Now;
            var prestamo = new Prestamo
            {
                Id = prestamoDto.Id,
                AutenticacionId = auth.Id,
                Autenticacion = auth,
                LibroId = prestamoDto.LibroId,
                Libro = libro,
                FechaPrestamo = prestamoDto.FechaPrestamo != null ? prestamoDto.FechaPrestamo : fechaToday.ToString("yyyy-MM-dd"),
                FechaDevolucion = prestamoDto.FechaDevolucion
            };

            List<IRegla> reglas = new List<IRegla>();

            var result = await prestamoDAO.ModificarAsync(id, prestamo, reglas);

            if (result)
            {
                return true;
            }
            else
            {
                customError = prestamoDAO.customError;
                return false;
            }
        }

        /// <summary>
        /// Permite borrar un prestamo por Id
        /// </summary>
        /// <param name="id">Id del prestamo</param>
        /// <returns></returns>
        public async Task<bool> BorrarAsync(string id)
        {
            var result = await prestamoDAO.BorrarAsync(id, new List<IRegla>(), "Prestamo");

            if (result)
            {
                return true;
            }
            else
            {
                customError = prestamoDAO.customError;
                return false;
            }
        }
    }
}
