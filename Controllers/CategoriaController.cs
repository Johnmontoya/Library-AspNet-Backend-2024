using Backend.DAO;
using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Backend.Resources;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

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
        private IValidator<CategoriaDto> _validator;
        private readonly LocService _locService;

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="context"></param>
        /// <param name="validator"></param>
        /// <param name="locService"></param>
        public CategoriaController(AppDbContext context, IValidator<CategoriaDto> validator, LocService locService)
        {
            _context = context;
            _locService = locService;
            _categoriaDAO = new CategoriaDAO(_context, _locService);
            _validator = validator;
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
            try
            {
                var categoria = await _categoriaDAO.ObtenerPorIdAsync(id);
                return Ok(categoria);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto
                {
                    title = String.Format(_locService.GetLocalizedString("NotFoundSpecific"), "Categoria"),
                    status = 404,
                    errors = new Dictionary<string, List<string>>
            {
                { "Categoria", new List<string> { ex.Message } }
            }
                });
            }
        }

        /// <summary>
        /// Permite almacenar una nueva categoria
        /// </summary>
        /// <param name="categoriaDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto>> Post([FromBody] CategoriaDto categoriaDto)
        {
            var result = await _validator.ValidateAsync(categoriaDto);

            if (!result.IsValid) 
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(modelState);
            }

            var categoria = await _categoriaDAO.AgregarAsync(categoriaDto);

            if (!categoria)
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
                title = String.Format(_locService.GetLocalizedString("Success"), "Categoria"),
                errors = null,
                status = 201
            });
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

            var result = await _validator.ValidateAsync(categoriaDto);

            if (!result.IsValid)
            {
                var modelState = new ModelStateDictionary();
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return ValidationProblem(modelState);
            }

            var categoria = await _categoriaDAO.ModificarAsync(id, categoriaDto);

            if (!categoria)
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
                title = String.Format(_locService.GetLocalizedString("Updated"), "Categoria"),
                errors = null,
                status = 200
            });
        }


        /// <summary>
        /// Permite eliminar una categoria por su ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto>> Delete([FromRoute] string id)
        {
            var result = await _categoriaDAO.BorrarAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponseDto
                {
                    title = _categoriaDAO.customError!.title!,
                    status = 404,
                    errors = _categoriaDAO.customError!.errors!
                });
            }

            return Ok(new ApiResponseDto
            {
                title = String.Format(_locService.GetLocalizedString("Deleted"), "Categoria"),
                errors = null,
                status = 200
            });
        }
    }
}
