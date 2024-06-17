using Backend.Database;
using Backend.Dtos;
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

        /// <summary>
        /// Constructor y validaciones de la clase autor
        /// </summary>
        /// <param name="context"></param>
        public AutorValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(u => u.Nombre)
                .NotNull().WithMessage("El campo nombre no puede estar vacio")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio")
                .MustAsync(BeUniqueNombre!).WithMessage("Un autor con ese nombre ya existe");
        }

        private async Task<bool> BeUniqueNombre(AutorDto autorDto, string nombre, CancellationToken cancellationToken)
        {
            return await _context.Autors
                .Where(c => c.Id != autorDto.Id)
                .AllAsync(n => n.Nombre != nombre, cancellationToken);
        }
    }
}
