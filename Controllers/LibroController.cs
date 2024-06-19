using Backend.DAO;
using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Backend.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Backend.Controllers
{
    /// <summary>
    /// Servicios para buscar, guardar, modificar o borrar los libros
    /// </summary>  
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController: ControllerBase
    {
        private readonly AppDbContext _context;
        private LibroDAO _libroDAO;
        private IValidator<LibroDto> _validator;
        private readonly LocService _locService;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="context"></param>
        /// <param name="validator"></param>
        /// <param name="locService"></param>
        public LibroController(AppDbContext context, IValidator<LibroDto> validator, LocService locService)
        {
            _context = context;
            _locService = locService;
            _validator = validator;
            _libroDAO = new LibroDAO(_context, _locService);            
        }

        /// <summary>
        /// Obtiene todos los libros almancenados
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<Libro>> GetAll()
        {
            var libros = await _libroDAO.ObtenerTodoAsync();
            return Ok(libros);
        }

        /// <summary>
        /// Obtiene un libro por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Autor>> GetId([FromRoute] string id)
        {
            try
            {
                var libros = await _libroDAO.ObtenerPorIdAsync(id);
                return Ok(libros);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Libro"),
                    status = 404,
                    errors = new Dictionary<string, List<string>>
            {
                { "Libro", new List<string> { ex.Message } }
            }
                });
            }
        }

        /// <summary>
        /// Permite almacenar un nuevo libro
        /// </summary>
        /// <param name="libroDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto>> Post([FromBody] LibroDto libroDto)
        {
            var validacion = await _validator.ValidateAsync(libroDto);

            if (!validacion.IsValid)
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in validacion.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(modelState);
            }

            var autor = await _context.Autors.FindAsync(libroDto.AutorId);

            if (autor == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Autor"),
                    status = 404,
                    errors = null
                });
            }

            var categoria = await _context.Categorias.FindAsync(libroDto.CategoriaId);

            if (categoria == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Categoria"),
                    status = 404,
                    errors = null
                });
            }

            var data = await _libroDAO.AgregarAsync(libroDto, categoria, autor);

            if (!data)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorRequest")),
                    status = 400,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("Success"), "Libro"),
                errors = null,
                status = 201

            });
        }

        /// <summary>
        /// Modifica un libro
        /// </summary>
        /// <param name="id"></param>
        /// <param name="libroDto"></param>
        /// <returns>Datos del libro</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto>> PutLibro([FromRoute] string id, [FromBody] LibroDto libroDto)
        {
            var libro = await _libroDAO.ObtenerPorIdAsync(id);

            if (libro == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Libro"),
                    status = 404,
                    errors = null
                });
            }

            var autor = await _context.Autors.FindAsync(libroDto.AutorId);

            if (autor == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Autor"),
                    status = 404,
                    errors = null
                });
            }

            var categoria = await _context.Categorias.FindAsync(libroDto.CategoriaId);

            if (categoria == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Categoria"),
                    status = 404,
                    errors = null
                });
            }

            var validacion = await _validator.ValidateAsync(libroDto);

            if (!validacion.IsValid)
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in validacion.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(modelState);
            }

            var result = await _libroDAO.ModificarAsync(id, libroDto, categoria, autor);

            if (!result)
            {
                return BadRequest(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("ErrorRequest")),
                    status = 400,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("Updated"), "Libro"),
                errors = null,
                status = 200
            });
        }

        /// <summary>
        /// Borrar un libro
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto>> DeleteLibro([FromRoute] string id)
        {
            var result = await _libroDAO.BorrarAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Libro"),
                    status = 404,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("Deleted"), "Libro"),
                errors = null,
                status = 200
            });
        }
    }
}
