using Backend.Models;
using Backend.Models.Entity_Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend.Database
{
    public class AppDbContext: IdentityDbContext<Authentication>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

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

        public virtual DbSet<Usuario> Usuarios { get; set; }

        public virtual DbSet<Authentication> Authentications { get; set; }

    }
}
