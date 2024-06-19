using Backend.Database;
using Backend.Dtos;
using Backend.Resources;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Validators
{
    /// <summary>
    /// Validador de autor
    /// </summary>
    public class AutorValidator : AbstractValidator<AutorDto>
    {
        private readonly AppDbContext _context;
        private readonly LocService _localizer;

        /// <summary>
        /// Constructor y validaciones de la clase autor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="localizer"></param>
        public AutorValidator(AppDbContext context, LocService localizer)
        {
            _context = context;
            _localizer = localizer;

            RuleFor(u => u.Nombre)
                .NotNull().WithMessage(String.Format(_localizer.GetLocalizedString("NotNull"), "nombre"))
                .NotEmpty().WithMessage(String.Format(_localizer.GetLocalizedString("NotEmpty"), "nombre"))
                .MustAsync(BeUniqueNombre!).WithMessage(String.Format(_localizer.GetLocalizedString("Duplicate"), "nombre"));
        }

        private async Task<bool> BeUniqueNombre(AutorDto autorDto, string nombre, CancellationToken cancellationToken)
        {
            return await _context.Autors
                .Where(c => c.Id != autorDto.Id)
                .AllAsync(n => n.Nombre != nombre, cancellationToken);
        }
    }
}
