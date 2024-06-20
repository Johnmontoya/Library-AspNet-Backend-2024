using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Models.Entity_Configuration
{
    /// <summary>
    /// LLaves foráneas e indices
    /// </summary>
    public class AutorConfiguration: IEntityTypeConfiguration<Autor>
    {
        /// <summary>
        /// Llaves foráneas e indices
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<Autor> builder) 
        {
            builder.HasMany(l => l.LibrosEscritos)
                .WithOne(a => a.Autor)
                .HasForeignKey(a => a.AutorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
