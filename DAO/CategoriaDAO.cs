using Backend.Core;
using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.DAO
{
    /// <summary>
    /// Funciones para acceso a los datos para las categorías
    /// </summary>
    public class CategoriaDAO
    {
        private readonly AppDbContext _context;
        private AccessDAO<Categoria> categoriaDAO;

        /// <summary>
        /// Mensaje de error personalizado
        /// </summary>
        public CustomError? customError;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="context"></param>
        public CategoriaDAO(AppDbContext context)
        {
            _context = context;
            categoriaDAO = new AccessDAO<Categoria>(_context);
        }

        /// <summary>
        /// Obtiene todas las categorias
        /// </summary>
        /// <returns></returns>
        public async Task<List<Categoria>> ObtenerTodoAsync()
        {
            return _context.Categorias.ToList();
        }

        /// <summary>
        /// Obtiene una categoría por su Id
        /// </summary>
        /// <param name="id">Id de la categoría</param>
        /// <returns></returns>
        public async Task<Categoria> ObtenerPorIdAsync(string id)
        {            
            var result = _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);            
            return result.Result!;
            
        }

        /// <summary>
        /// Permite registrar una nueva categoria
        /// </summary>
        public async Task<bool> AgregarAsync(CategoriaDto categoriaDto)
        {
            var categoria = new Categoria
            {
                Clave = categoriaDto.Clave,
                Nombre = categoriaDto.Nombre
            };

            List<IRegla> reglas = new List<IRegla>();

            var result = await categoriaDAO.AgregarAsync(categoria, reglas);

            if (result)
            {
                return true;
            }
            else
            {
                customError = categoriaDAO.customError;
                return false;
            }
        }

        /// <summary>
        /// Modifica una categoria
        /// </summary>
        /// <param name="id">Id a buscar</param>
        /// <param name="categoriaDto">Datos de la categoria</param>
        /// <returns></returns>
        public async Task<bool> ModificarAsync(string id, CategoriaDto categoriaDto)
        {
            var categoria = new Categoria
            {
                Id = categoriaDto.Id,
                Clave = categoriaDto.Clave,
                Nombre = categoriaDto.Nombre
            };

            List<IRegla> reglas = new List<IRegla>();

            var result = await categoriaDAO.ModificarAsync(id, categoria, reglas);

            if (result)
            {
                return true;
            }
            else
            {
                customError = categoriaDAO.customError;
                return false;
            }
        }

        /// <summary>
        /// Permite borrar una categoría por Id
        /// </summary>
        /// <param name="id">Id de la categoría</param>
        /// <returns></returns>
        public async Task<bool> BorrarAsync(string id)
        {
            var result = await categoriaDAO.BorrarAsync(id, new List<IRegla>(), "Categoria");

            if (result)
            {
                return true;
            }
            else
            {
                customError = categoriaDAO.customError;
                return false;
            }
        }
    }
}
