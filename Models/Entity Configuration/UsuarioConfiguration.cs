using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Models.Entity_Configuration
{
    /// <summary>
    /// Configuracion de la tabla usuario
    /// </summary>
    public class UsuarioConfiguration: IEntityTypeConfiguration<Usuario>
    {
        /// <summary>
        /// Creacion de foraneas e indices
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasOne(e => e.Authentication)
                .WithOne(s => s.Usuario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
 