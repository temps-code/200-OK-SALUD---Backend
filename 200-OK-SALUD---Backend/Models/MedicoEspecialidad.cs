using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _200_OK_SALUD___Backend.Models
{
    public class MedicoEspecialidad
    {
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(Medico))]
        public int MedicoId { get; set; }
        public Medico Medico { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(Especialidad))]
        public int EspecialidadId { get; set; }
        public Especialidad Especialidad { get; set; }

        // Soft-delete lógico
        public bool IsActive { get; set; } = true;

        // Auditoría
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
