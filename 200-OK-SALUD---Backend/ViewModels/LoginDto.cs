using System.ComponentModel.DataAnnotations;

namespace _200_OK_SALUD___Backend.ViewModels
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
