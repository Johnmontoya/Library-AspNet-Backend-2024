using Backend.Database;
using Backend.Dtos;
using Backend.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.Rules
{
    /// <summary>
    /// Validador de categoria
    /// </summary>
    public class CategoriaValidator: AbstractValidator<CategoriaDto>
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Constructor y validaciones de la clase Categoria
        /// </summary>
        /// <param name="context"></param>
        public CategoriaValidator(AppDbContext context)
        {
            _context = context;

            RuleFor(c => c.Clave)
                .NotNull().WithMessage("El campo clave no puede estar vacio")
                .NotEmpty().WithMessage("El campo clave no puede estar vacio")
                .LessThan(100).WithMessage("La clave no puede ser mayor que 100")
                .GreaterThanOrEqualTo(0).WithMessage("La clave no puede ser menor a 0")
                .MustAsync(BeUniqueClave!).WithMessage("Una categoria con esa clave ya existe");

            RuleFor(c => c.Nombre)
                .NotNull().WithMessage("El campo nombre no puede estar vacio")
                .NotEmpty().WithMessage("El campo nombre no puede estar vacio")
                .MustAsync(BeUniqueNombre!).WithMessage("Una categoria con ese nombre ya existe");
                
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
