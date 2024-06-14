using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Models.Entity_Configuration
{
    public class UsuarioConfiguration: IEntityTypeConfiguration<Authentication>
    {
        public void Configure(EntityTypeBuilder<Authentication> builder)
        {
            builder.HasIndex(c => new { c.UsuarioId })
                .HasDatabaseName("UI_Usuario")
                .IsUnique();
        }
    }
}
