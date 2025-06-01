using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _200_OK_SALUD___Backend.Models
{
    public class Medico
    {
        [Key]
        public int MedicoId { get; set; }

        [Required]
        [ForeignKey(nameof(Usuario))]
        public string UsuarioId { get; set; }
        public ApplicationUser Usuario { get; set; }

        [Required, MaxLength(50)]
        public string Nombre { get; set; }

        [Required, MaxLength(50)]
        public string Apellido { get; set; }

        [MaxLength(20)]
        public string Telefono { get; set; }

        // ESTA LÍNEA es necesaria para la relación 1:N con Cita
        public ICollection<Cita> Citas { get; set; }

        public ICollection<MedicoEspecialidad> MedicoEspecialidades { get; set; }
    }
}
