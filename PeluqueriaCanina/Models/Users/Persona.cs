using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDePeluquero;
using PeluqueriaCanina.Models.ClasesDeTurno;
using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models.Users
{
    public abstract class Persona : Usuario
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        [StringLength(8)]
        public string Dni { get; set; } = string.Empty;
        [Required]
        public DateTime FechaDeNacimiento { get; set; }
        public List<Mascota> Mascotas { get; set; } = new List<Mascota>();
        public List<Valoracion> ValoracionesRealizadas { get; set; } = new();
        public List<Valoracion> ValoracionesRecibidas { get; set; } = new();
        public List<Jornada> Jornadas { get; set; } = new List<Jornada>();
        public List<Turno> Turnos { get; set; } = new List<Turno> { };
    }
}
