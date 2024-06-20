using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Backend.Models.Entity_Configuration
{
    /// <summary>
    /// LLaves foráneas e indices
    /// </summary>
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {
        /// <summary>
        /// Llaves foráneas e indices
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.HasMany(l => l.LibrosEscritos)
                .WithOne(a => a.Categoria)
                .HasForeignKey(a => a.CategoriaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
