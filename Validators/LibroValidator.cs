using Backend.Database;
using Backend.Dtos;
using Backend.Resources;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Validators
{
    /// <summary>
    /// Validador de libro
    /// </summary>
    public class LibroValidator : AbstractValidator<LibroDto>
    {
        private readonly AppDbContext _context;
        private readonly LocService _localizer;

        /// <summary>
        /// Constructor y validaciones de la clase libro
        /// </summary>
        /// <param name="context"></param>
        /// <param name="localizer"></param>
        public LibroValidator(AppDbContext context, LocService localizer)
        {
            _context = context;
            _localizer = localizer;

            RuleFor(u => u.Nombre)
                .NotNull().WithMessage(String.Format(_localizer.GetLocalizedString("NotNull"), "nombre"))
                .NotEmpty().WithMessage(String.Format(_localizer.GetLocalizedString("NotEmpty"), "nombre"))
                .MustAsync(BeUniqueNombre!).WithMessage(String.Format(_localizer.GetLocalizedString("Duplicate"), "nombre"));
        }

        private async Task<bool> BeUniqueNombre(LibroDto libroDto, string nombre, CancellationToken cancellationToken)
        {
            return await _context.Libros
                .Where(c => c.Id != libroDto.Id)
                .AllAsync(n => n.Nombre != nombre, cancellationToken);
        }
    }
}
