using Backend.Database;
using Backend.Dtos;
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

        /// <summary>
        /// Constructor y validaciones de la clase Usuario
        /// </summary>
        /// <param name="context"></param>
        public UsuarioValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(u => u.DNI)
                .NotNull().WithMessage("El campo DNI no puede estar vacio")
                .NotEmpty().WithMessage("El campo DNI no puede estar vacio")
                .MustAsync(BeUniqueDNI!).WithMessage("Un usuario con ese DNI ya existe");
        }

        private async Task<bool> BeUniqueDNI(UsuarioDto usuarioDto, int dni, CancellationToken cancellationToken)
        {
            return await _context.Usuarios
                .Where(c => c.Id != usuarioDto.Id)
                .AllAsync(n => n.DNI != dni, cancellationToken);
        }
    }
}
