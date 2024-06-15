using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    [Display(Name = "Authentication")]
    public class Authentication: IdentityUser
    {
        public Usuario? Usuario { get; set; }
        public string? OTPSecurity { get; set; }
    }
}
