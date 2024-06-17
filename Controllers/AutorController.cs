using Backend.DAO;
using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Backend.Controllers
{
    /// <summary>
    /// Servicios para buscar, guardar, modificar o borrar los autores
    /// </summary>  
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private AutorDAO _autorDAO;
        private IValidator<AutorDto> _validator;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="context"></param>
        /// <param name="validator"></param>
        public AutorController(AppDbContext context, IValidator<AutorDto> validator)
        {
            _context = context;
            _autorDAO = new AutorDAO(_context);
            _validator = validator;
        }

        /// <summary>
        /// Obtiene todos los autores almancenadas
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<Autor>> GetAll()
        {
            var autores = await _autorDAO.ObtenerTodoAsync();
            return Ok(autores);
        }

        /// <summary>
        /// Obtiene un autor por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Autor>> GetId([FromRoute] string id)
        {
            try
            {
                var autores = await _autorDAO.ObtenerPorIdAsync(id);
                return Ok(autores);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto
                {
                    title = "Autor no encontrado",
                    status = 404,
                    errors = new Dictionary<string, List<string>>
            {
                { "Autor", new List<string> { ex.Message } }
            }
                });
            }
        }

        /// <summary>
        /// Permite almacenar un nuevo autor
        /// </summary>
        /// <param name="autorDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto>> Post([FromBody] AutorDto autorDto)
        {
            var result = await _validator.ValidateAsync(autorDto);

            if (!result.IsValid)
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(modelState);
            }

            var categoria = await _autorDAO.AgregarAsync(autorDto);

            if (!categoria)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = "Hubo un problema al guardar el autor",
                    status = 400,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = "Autor Guardado",
                errors = null,
                status = 201

            });
        }

        /// <summary>
        /// Permite actualizar un autor por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="autorDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Categoria>> Put([FromRoute] string id, [FromBody] AutorDto autorDto)
        {
            var data = await _autorDAO.ObtenerPorIdAsync(id);

            var result = await _validator.ValidateAsync(autorDto);

            if (!result.IsValid)
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(modelState);
            }

            var categoria = await _autorDAO.ModificarAsync(id, autorDto);

            if (!categoria)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = "Hubo un problema al actualizar el autor",
                    status = 400,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = "Autor Actualizado",
                errors = null,
                status = 200
            });
        }

        /// <summary>
        /// Permite eliminar un autor por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto>> Delete([FromRoute] string id)
        {
            var result = await _autorDAO.BorrarAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponseDto
                {
                    title = _autorDAO.customError!.title!,
                    status = 404,
                    errors = _autorDAO.customError!.errors!
                });
            }

            return Ok(new ApiResponseDto
            {
                title = "Autor eliminado",
                errors = null,
                status = 200
            });
        }
    }
}
