using System.ComponentModel.DataAnnotations;

namespace _200_OK_SALUD___Backend.ViewModels
{
    public class CrearActualizarEspecialidadDto
    {
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }
    }
}
