using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _200_OK_SALUD___Backend.Models
{
    public class Notificacion
    {
        [Key]
        public int NotificacionId { get; set; }

        [Required]
        [ForeignKey(nameof(Cita))]
        public int CitaId { get; set; }
        public Cita Cita { get; set; }

        public DateTime? FechaEnvio { get; set; }

        [MaxLength(10)]
        public string Metodo { get; set; } // "email","sms","app"

        [MaxLength(10)]
        public string Estado { get; set; } // "enviado","fallido"

        [Required]
        public bool EsAlertaTurno { get; set; } = false;
    }
}
