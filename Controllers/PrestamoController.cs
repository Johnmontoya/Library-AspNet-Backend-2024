using Backend.DAO;
using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Backend.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;

namespace Backend.Controllers
{
    /// <summary>
    /// Servicios para buscar, guardar, modificar o borrar los prestamos
    /// </summary>  
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PrestamoController: ControllerBase
    {
        private readonly AppDbContext _context;
        private PrestamoDAO _prestamoDAO;
        private readonly UserManager<Authentication> _userManager;
        private readonly LocService _locService;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userManager"></param>
        /// <param name="locService"></param>
        public PrestamoController(AppDbContext context, UserManager<Authentication> userManager, LocService locService)
        {
            _context = context;
            _locService = locService;            
            _userManager = userManager;
            _prestamoDAO = new PrestamoDAO(_context, _locService);
        }

        /// <summary>
        /// Obtiene todos los prestamos almancenados
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<ActionResult<Prestamo>> GetAll()
        {
            var prestamos = await _prestamoDAO.ObtenerTodoAsync();
            return Ok(prestamos);
        }

        /// <summary>
        /// Obtiene un prestamo por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Prestamo>> GetId([FromRoute] string id)
        {
            try
            {
                var prestamo = await _prestamoDAO.ObtenerPorIdAsync(id);
                return Ok(prestamo);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Prestamo"),
                    status = 404,
                    errors = new Dictionary<string, List<string>>
            {
                { "Prestamo", new List<string> { ex.Message } }
            }
                });
            }
        }

        /// <summary>
        /// Permite almacenar un nuevo prestamo
        /// </summary>
        /// <param name="prestamoDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto>> Post([FromBody] PrestamoDto prestamoDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("Unauthorized")),
                    status = 401,
                    errors = null
                });
            }

            var auth = await _userManager.FindByNameAsync(currentUserId!);

            if (auth == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Usuario"),
                    status = 404,
                    errors = null
                });
            }

            var libro = await _context.Libros.FindAsync(prestamoDto.LibroId);

            if (libro == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Libro"),
                    status = 404,
                    errors = null
                });
            }

            var data = await _prestamoDAO.AgregarAsync(prestamoDto, auth, libro);

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
                title = String.Format(_locService.GetLocalizedString("Success"), "Prestamo"),
                errors = null,
                status = 201

            });
        }

        /// <summary>
        /// Permite actualizar un nuevo prestamo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prestamoDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto>> Post([FromRoute] string id, [FromBody] PrestamoDto prestamoDto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentUserId == null)
            {
                return Unauthorized(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("Unauthorized")),
                    status = 401,
                    errors = null
                });
            }

            var auth = await _userManager.FindByNameAsync(currentUserId!);

            if (auth == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Usuario"),
                    status = 404,
                    errors = null
                });
            }

            var libro = await _context.Libros.FindAsync(prestamoDto.LibroId);

            if (libro == null)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Libro"),
                    status = 404,
                    errors = null
                });
            }

            var data = await _prestamoDAO.ModificarAsync(id, prestamoDto, auth, libro);

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
                title = String.Format(_locService.GetLocalizedString("Updated"), "Prestamo"),
                errors = null,
                status = 201

            });
        }

        /// <summary>
        /// Borrar un prestamo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto>> DeletePrestamo([FromRoute] string id)
        {
            var result = await _prestamoDAO.BorrarAsync(id);
            if (!result)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Prestamo"),
                    status = 404,
                    errors = null
                });
            }

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("Deleted"), "Prestamo"),
                errors = null,
                status = 200
            });
        }
    }
}
