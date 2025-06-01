using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _200_OK_SALUD___Backend.Models
{
    public class CitaPrioridad
    {
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(Cita))]
        public int CitaId { get; set; }
        public Cita Cita { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(ReglaPrioridad))]
        public int ReglaId { get; set; }
        public ReglaPrioridad ReglaPrioridad { get; set; }
    }
}
