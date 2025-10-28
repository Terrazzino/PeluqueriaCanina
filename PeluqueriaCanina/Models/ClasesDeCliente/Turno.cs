using PeluqueriaCanina.Models.Users;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDePeluquero;
using PeluqueriaCanina.Models.ClasesDeAdministrador;

namespace PeluqueriaCanina.Models.ClasesDeTurno
{
    public class Turno
    {
        public int Id { get; set; }

        public int MascotaId { get; set; }
        public Mascota Mascota { get; set; }

        public int? PeluqueroId { get; set; }
        public Peluquero? Peluquero { get; set; }

        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }

        public DateTime FechaHora { get; set; }
        public TimeSpan Duracion { get; set; }

        public EstadoTurno Estado { get; set; } = EstadoTurno.PendingPayment; // más adelante esto será manejado con el patrón State

        public decimal Precio { get; set; }

        public DateTime HoraInicio => FechaHora;
        public DateTime HoraFin => FechaHora.Add(Duracion);

    }
}
