using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDePeluquero;
using PeluqueriaCanina.Models.ClasesDeTurno;

namespace PeluqueriaCanina.Models.Users
{
    public class Peluquero : Persona
    {
        public List<Jornada> Jornadas { get; set; } = new List<Jornada>();
        public List<Turno> Turnos { get; set; } = new List<Turno> { };
        public List<Valoracion> Valoraciones { get; set; } = new();

    }
}
