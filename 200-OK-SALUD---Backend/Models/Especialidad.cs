using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _200_OK_SALUD___Backend.Models
{
    public class Especialidad
    {
        [Key]
        public int EspecialidadId { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        // Soft-delete lógico
        public bool IsActive { get; set; } = true;

        // Auditoría
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<MedicoEspecialidad> MedicoEspecialidades { get; set; }
    }
}
