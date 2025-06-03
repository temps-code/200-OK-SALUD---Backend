using System;
using System.ComponentModel.DataAnnotations;

namespace _200_OK_SALUD___Backend.Models
{
    public class Cita
    {
        [Key]
        public int CitaId { get; set; }

        [Required]
        public int PacienteId { get; set; }
        public Paciente Paciente { get; set; }

        [Required]
        public int MedicoId { get; set; }
        public Medico Medico { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required, MaxLength(15)]
        public string Estado { get; set; }

        [Required]
        public decimal PuntajePrioridad { get; set; }

        [Required]
        public int NumeroTurno { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
