using Backend.Database;
using Backend.Dtos;
using Backend.Resources;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Backend.Validators
{
    /// <summary>
    /// Validador de usuario
    /// </summary>
    public class UsuarioValidator : AbstractValidator<UsuarioDto>
    {
        private readonly AppDbContext _context;
        private readonly LocService _localizer;

        /// <summary>
        /// Constructor y validaciones de la clase Usuario
        /// </summary>
        /// <param name="context"></param>
        /// <param name="locService"></param>
        public UsuarioValidator(AppDbContext context, LocService locService)
        {
            _context = context;
            _localizer = locService;

            RuleFor(u => u.DNI)
                .NotNull().WithMessage(String.Format(_localizer.GetLocalizedString("NotNull"), "DNI"))
                .NotEmpty().WithMessage(String.Format(_localizer.GetLocalizedString("NotEmpty"), "DNI"))
                .MustAsync(BeUniqueDNI!).WithMessage(String.Format(_localizer.GetLocalizedString("Duplicate"), "DNI"));
        }

        private async Task<bool> BeUniqueDNI(UsuarioDto usuarioDto, int dni, CancellationToken cancellationToken)
        {
            return await _context.Usuarios
                .Where(c => c.Id != usuarioDto.Id)
                .AllAsync(n => n.DNI != dni, cancellationToken);
        }
    }
}
