using Backend.Core;
using Backend.Core.Errors;
using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Backend.Resources;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAO
{
    /// <summary>
    /// Funciones para acceso a los datos para los libros
    /// </summary>
    public class LibroDAO
    {
        private readonly AppDbContext _context;
        private AccessDAO<Libro> libroDAO;
        private readonly LocService _locService;

        /// <summary>
        /// Mensaje de error personalizado
        /// </summary>
        public CustomError? customError;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="context"></param>
        /// <param name="locService"></param>
        public LibroDAO(AppDbContext context, LocService locService)
        {
            _context = context;
            _locService = locService;
            libroDAO = new AccessDAO<Libro>(_context, _locService);
        }

        /// <summary>
        /// Obtiene todos los libros
        /// </summary>
        /// <returns></returns>
        public async Task<List<Libro>> ObtenerTodoAsync()
        {
            return _context.Libros.ToList();
        }

        /// <summary>
        /// Obtiene un libro por su Id
        /// </summary>
        /// <param name="id">Id del libro</param>
        /// <returns></returns>
        public async Task<Libro> ObtenerPorIdAsync(string id)
        {
            var result = _context.Libros.FirstOrDefaultAsync(c => c.Id == id);
            return result.Result!;

        }

        /// <summary>
        /// Permite registrar un nuevo libro
        /// </summary>
        public async Task<bool> AgregarAsync(LibroDto libroDto, Categoria categoria, Autor autor)
        {
            var libro = new Libro
            {
                Nombre = libroDto.Nombre,
                Editorial = libroDto.Editorial,
                CategoriaId = libroDto.CategoriaId,
                Categoria = categoria,
                AutorId = libroDto.AutorId,
                Autor = autor
            };

            List<IRegla> reglas = new List<IRegla>();

            var result = await libroDAO.AgregarAsync(libro, reglas);

            if (result)
            {
                return true;
            }
            else
            {
                customError = libroDAO.customError;
                return false;
            }
        }

        /// <summary>
        /// Modifica un libro
        /// </summary>
        /// <param name="id">Id a buscar</param>
        /// <param name="libroDto">Datos del libro</param>
        /// <param name="autor">Datos del autor</param>
        /// <param name="categoria">Datos de categoria</param>
        /// <returns>Mensaje de validacion</returns>
        public async Task<bool> ModificarAsync(string id, LibroDto libroDto, Categoria categoria, Autor autor)
        {
            var book = new Libro
            {
                Id = id,
                CategoriaId = libroDto.CategoriaId,
                Categoria = categoria,
                Nombre = libroDto.Nombre,
                Editorial = libroDto.Editorial,
                AutorId = libroDto.AutorId,
                Autor = autor
            };

            List<IRegla> reglas = new List<IRegla>();

            var result = await libroDAO.ModificarAsync(id, book, reglas);
            if (result)
            {
                return true;
            }
            else
            {
                customError = libroDAO.customError;
                return false;
            }
        }

        /// <summary>
        /// Permite borrar un libro por Id
        /// </summary>
        /// <param name="id">Id del libro</param>
        /// <returns></returns>
        public async Task<bool> BorrarAsync(string id)
        {
            var result = await libroDAO.BorrarAsync(id, new List<IRegla>(), "Libro");

            if (result)
            {
                return true;
            }
            else
            {
                customError = libroDAO.customError;
                return false;
            }
        }
    }
}
