// ViewModels/PacienteDto.cs
using System;

namespace _200_OK_SALUD___Backend.ViewModels
{
    public class PacienteDto
    {
        public int PacienteId { get; set; }
        public string UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public string Telefono { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
