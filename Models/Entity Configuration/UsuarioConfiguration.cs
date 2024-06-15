using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Models.Entity_Configuration
{
    public class UsuarioConfiguration: IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasOne(e => e.Authentication)
                .WithOne(s => s.Usuario)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
 