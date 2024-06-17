using Backend.Database;
using Backend.Dtos;
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

        /// <summary>
        /// Constructor y validaciones de la clase libro
        /// </summary>
        /// <param name="context"></param>
        public LibroValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(u => u.Nombre)
                .NotNull().WithMessage("El campo nombre no puede estar vacio")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio")
                .MustAsync(BeUniqueNombre!).WithMessage("Un libro con ese nombre ya existe");
        }

        private async Task<bool> BeUniqueNombre(LibroDto libroDto, string nombre, CancellationToken cancellationToken)
        {
            return await _context.Libros
                .Where(c => c.Id != libroDto.Id)
                .AllAsync(n => n.Nombre != nombre, cancellationToken);
        }
    }
}
