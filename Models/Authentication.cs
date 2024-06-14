using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    [Display(Name = "Authentication")]
    public class Authentication: IdentityUser
    {
        public string? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
