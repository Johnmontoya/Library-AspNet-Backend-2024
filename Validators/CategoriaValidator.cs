using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using Backend.Resources;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Rules
{
    /// <summary>
    /// Validador de categoria
    /// </summary>
    public class CategoriaValidator: AbstractValidator<CategoriaDto>
    {
        private readonly AppDbContext _context;
        private readonly LocService _localizer;

        /// <summary>
        /// Constructor y validaciones de la clase Categoria
        /// </summary>
        /// <param name="context"></param>
        /// <param name="locService"></param>
        public CategoriaValidator(AppDbContext context, LocService locService)
        {
            _context = context;
            _localizer = locService;

            RuleFor(c => c.Clave)
                .NotNull().WithMessage(String.Format(_localizer.GetLocalizedString("NotNull"), "clave"))
                .NotEmpty().WithMessage(String.Format(_localizer.GetLocalizedString("NotEmpty"), "clave"))
                .LessThan(100).WithMessage(String.Format(_localizer.GetLocalizedString("LessThan"), "clave", "100"))
                .GreaterThanOrEqualTo(0).WithMessage(String.Format(_localizer.GetLocalizedString("GreaterThan"), "clave", "0"))
                .MustAsync(BeUniqueClave!).WithMessage(String.Format(_localizer.GetLocalizedString("Duplicate"), "clave"));

            RuleFor(c => c.Nombre)
                .NotNull().WithMessage(String.Format(_localizer.GetLocalizedString("NotNull"), "nombre"))
                .NotEmpty().WithMessage(String.Format(_localizer.GetLocalizedString("NotEmpty"), "nombre"))
                .MustAsync(BeUniqueNombre!).WithMessage(String.Format(_localizer.GetLocalizedString("Duplicate"), "nombre"));
                
        }
        private async Task<bool> BeUniqueClave(CategoriaDto categoriaDto, int clave, CancellationToken cancellationToken)
        {            
            return await _context.Categorias
                .Where(c => c.Id != categoriaDto.Id)
                .AllAsync(n => n.Clave != clave, cancellationToken);
        }

        private async Task<bool> BeUniqueNombre(CategoriaDto categoriaDto, string nombre, CancellationToken cancellationToken)
        {           
            return await _context.Categorias
                .Where(n => n.Id != categoriaDto.Id)
                .AllAsync(n => n.Nombre != nombre, cancellationToken);
        }
    }
}
