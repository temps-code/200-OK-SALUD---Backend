using System.ComponentModel.DataAnnotations;

namespace _200_OK_SALUD___Backend.ViewModels
{
    public class ActualizarMedicoDto
    {
        [Required, MaxLength(50)]
        public string Nombre { get; set; }

        [Required, MaxLength(50)]
        public string Apellido { get; set; }

        [MaxLength(20)]
        public string Telefono { get; set; }
    }
}
