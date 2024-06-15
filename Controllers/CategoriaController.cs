using Backend.DAO;
using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    /// <summary>
    /// Servicios para buscar, guardar, modificar o borrar las categorias
    /// </summary>
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController: ControllerBase
    {
        private readonly AppDbContext _context;
        private CategoriaDAO _categoriaDAO;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="context"></param>
        public CategoriaController(AppDbContext context)
        {
            _context = context;
            _categoriaDAO = new CategoriaDAO(_context);
        }

        /// <summary>
        /// Obtiene todas la categorias almancenadas
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<Categoria>> GetAll()
        {
            var categorias = await _categoriaDAO.ObtenerTodoAsync();
            return Ok(categorias);
        }

        /// <summary>
        /// Obtiene una categoria por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Categoria>> GetId([FromRoute] string id)
        {
            var categoria = await _categoriaDAO.ObtenerPorIdAsync(id);
            return Ok(categoria);
        }

        /// <summary>
        /// Permite almacenar una nueva categoria
        /// </summary>
        /// <param name="categoriaDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Categoria>> Post([FromBody] CategoriaDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoria = await _categoriaDAO.AgregarAsync(categoriaDto);

            if (!categoria)
            {
                return BadRequest("Error al guardar la categoria");
            }

            return Ok("Categoria guardada");
        }

        /// <summary>
        /// Permite actualizar una categoria por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoriaDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Categoria>> Put([FromRoute] string id, [FromBody] CategoriaDto categoriaDto)
        {
            var data = await _categoriaDAO.ObtenerPorIdAsync(id);

            if(data == null)
            {
                return BadRequest("Categoria no encontrada");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _categoriaDAO.ModificarAsync(id, categoriaDto);

            if (!result)
            {
                return BadRequest("Error al actualizar la categoria");
            }

            return Ok("Categoria Actualizada");
        }


        /// <summary>
        /// Permite eliminar una categoria por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> Delete([FromRoute] string id)
        {
            var result = await _categoriaDAO.BorrarAsync(id);

            if (!result)
            {
                return BadRequest("Error al eliminar la categoria");
            }

            return Ok("Categoria eliminada");
        }

    }
}
