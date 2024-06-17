using Backend.Core;
using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAO
{
    /// <summary>
    /// Funciones para acceso a los datos para los autores
    /// </summary>
    public class AutorDAO
    {
        private readonly AppDbContext _context;
        private AccessDAO<Autor> autorDAO;

        /// <summary>
        /// Mensaje de error personalizado
        /// </summary>
        public CustomError? customError;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="context"></param>
        public AutorDAO(AppDbContext context)
        {
            _context = context;
            autorDAO = new AccessDAO<Autor>(_context);
        }

        /// <summary>
        /// Obtiene todos los autores
        /// </summary>
        /// <returns></returns>
        public async Task<List<Autor>> ObtenerTodoAsync()
        {
            return _context.Autors.ToList();
        }

        /// <summary>
        /// Obtiene un autor por su Id
        /// </summary>
        /// <param name="id">Id del autor</param>
        /// <returns></returns>
        public async Task<Autor> ObtenerPorIdAsync(string id)
        {
            var result = _context.Autors.FirstOrDefaultAsync(c => c.Id == id);
            return result.Result!;

        }

        /// <summary>
        /// Permite registrar un nuevo autor
        /// </summary>
        public async Task<bool> AgregarAsync(AutorDto autorDto)
        {
            var autor = new Autor
            {
                Nombre = autorDto.Nombre,
                Nacionalidad = autorDto.Nacionalidad
            };

            List<IRegla> reglas = new List<IRegla>();

            var result = await autorDAO.AgregarAsync(autor, reglas);

            if (result)
            {
                return true;
            }
            else
            {
                customError = autorDAO.customError;
                return false;
            }
        }

        /// <summary>
        /// Modifica un autor
        /// </summary>
        /// <param name="id">Id a buscar</param>
        /// <param name="autorDto">Datos del autor</param>
        /// <returns></returns>
        public async Task<bool> ModificarAsync(string id, AutorDto autorDto)
        {
            var autor = new Autor
            {
                Id = autorDto.Id,
                Nombre = autorDto.Nombre,
                Nacionalidad = autorDto.Nacionalidad
            };

            List<IRegla> reglas = new List<IRegla>();

            var result = await autorDAO.ModificarAsync(id, autor, reglas);

            if (result)
            {
                return true;
            }
            else
            {
                customError = autorDAO.customError;
                return false;
            }
        }

        /// <summary>
        /// Permite borrar un autor por Id
        /// </summary>
        /// <param name="id">Id del autor</param>
        /// <returns></returns>
        public async Task<bool> BorrarAsync(string id)
        {
            var result = await autorDAO.BorrarAsync(id, new List<IRegla>(), "Autor");

            if (result)
            {
                return true;
            }
            else
            {
                customError = autorDAO.customError;
                return false;
            }
        }
    }
}
