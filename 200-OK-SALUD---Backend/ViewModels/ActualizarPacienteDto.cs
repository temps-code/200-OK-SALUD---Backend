using System;
using System.ComponentModel.DataAnnotations;

namespace _200_OK_SALUD___Backend.ViewModels
{
    public class ActualizarPacienteDto
    {
        [Required, MaxLength(50)]
        public string Nombre { get; set; }

        [Required, MaxLength(50)]
        public string Apellido { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [MaxLength(1)]
        public string Genero { get; set; }

        [MaxLength(20)]
        public string Telefono { get; set; }
    }
}
