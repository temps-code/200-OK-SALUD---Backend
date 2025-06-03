using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _200_OK_SALUD___Backend.Models
{
    public class HistorialMedico
    {
        [Key]
        public int HistorialId { get; set; }

        [Required]
        [ForeignKey(nameof(Paciente))]
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        public string Descripcion { get; set; }

        // Soft-delete lógico
        public bool IsActive { get; set; } = true;

        // Auditoría
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
