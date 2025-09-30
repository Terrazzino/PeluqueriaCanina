using Microsoft.AspNetCore.Mvc.ModelBinding;
using PeluqueriaCanina.Models.ClasesDeTurno;
using PeluqueriaCanina.Models.Users;

namespace PeluqueriaCanina.Models.ClasesDeCliente
{
    public class Mascota
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Raza { get; set; }
        public DateTime FechaDeNacimiento { get; set; }
        public double Peso { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int TurnoId { get; set; }
        public Turno? Turno { get; set; }
    }
}
