using System.ComponentModel.DataAnnotations;

namespace _200_OK_SALUD___Backend.Models
{
    public class Especialidad
    {
        [Key]
        public int EspecialidadId { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        // Relación inversa a Médicos
        public ICollection<MedicoEspecialidad> MedicoEspecialidades { get; set; }
    }
}
