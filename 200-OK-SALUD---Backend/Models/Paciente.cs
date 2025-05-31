using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _200_OK_SALUD___Backend.Models
{
    public class Paciente
    {
        [Key]
        public int PacienteId { get; set; }

        [Required]
        [ForeignKey(nameof(Usuario))]
        public string UsuarioId { get; set; }
        public ApplicationUser Usuario { get; set; }

        [Required, MaxLength(50)]
        public string Nombre { get; set; }

        [Required, MaxLength(50)]
        public string Apellido { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [MaxLength(1)]
        public string Genero { get; set; }  // 'M', 'F' u 'O'

        [MaxLength(20)]
        public string Telefono { get; set; }

        // ESTA LÍNEA es la que faltaba: colección de navegación a Cita
        public ICollection<Cita> Citas { get; set; }
    }
}
