using System;

namespace _200_OK_SALUD___Backend.ViewModels
{
    public class MedicoDto
    {
        public int MedicoId { get; set; }
        public string UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Telefono { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
