using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    [Display(Name = "Usuario")]
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "Varchar(255)")]
        public string? Id { get; set; }
        public int DNI { get; set; }
        public string FechaNacimiento { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;

        [ForeignKey("Authentication")]
        public string? AuthenticationId { get; set; }
        public Authentication? Authentication { get; set; }

    }
}
