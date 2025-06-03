using System.ComponentModel.DataAnnotations;

namespace _200_OK_SALUD___Backend.Models
{
    public class ReglaPrioridad
    {
        [Key]
        public int ReglaId { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        [Required, MaxLength(255)]
        public string Criterio { get; set; } // p.ej. "edad>65", "sintomas criticos"

        [Required]
        public decimal Peso { get; set; } // DECIMAL(5,2)

        // Soft-delete lógico
        public bool IsActive { get; set; } = true;

        // Auditoría
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
