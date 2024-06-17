using Backend.Models;
using Backend.Models.Entity_Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database
{
    /// <summary>
    /// Contexto de la base de datos
    /// </summary>
    public class AppDbContext: IdentityDbContext<Authentication>
    {
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="options"></param>
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        /// <summary>
        /// Creacion de los modelos
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "User",
                NormalizedName = "USER"
            });
            builder.ApplyConfiguration(new UsuarioConfiguration());
            base.OnModelCreating(builder);
        }

        /// <summary>
        /// Tabla usuarios
        /// </summary>
        public virtual DbSet<Usuario> Usuarios { get; set; }

        /// <summary>
        /// Tabla de autenticacion (aspnetusers)
        /// </summary>
        public virtual DbSet<Authentication> Authentications { get; set; }

        /// <summary>
        /// Tabla de categorias
        /// </summary>
        public virtual DbSet<Categoria> Categorias { get; set; }

        /// <summary>
        /// Tabla de autores
        /// </summary>
        public virtual DbSet<Autor> Autors { get; set; }

    }
}
