using System.ComponentModel.DataAnnotations;

namespace _200_OK_SALUD___Backend.ViewModels
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "La contraseña y su confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [RegularExpression("Administrador|Medico|Paciente", ErrorMessage = "El rol debe ser Administrador, Medico o Paciente.")]
        public string Role { get; set; }
    }
}
