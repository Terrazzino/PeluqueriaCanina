using PeluqueriaCanina.Models.ClasesDeAdministrador;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDePeluquero;
using PeluqueriaCanina.Models.Users;

namespace PeluqueriaCanina.Models.ClasesDeTurno
{
    public class Turno
    {
        public int Id { get; set; }

        public int MascotaId { get; set; }
        public Mascota Mascota { get; set; }

        public int? PeluqueroId { get; set; }
        public Persona? Peluquero { get; set; }

        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }

        public DateTime FechaHora { get; set; }
        public TimeSpan Duracion { get; set; }

        public EstadoTurno Estado { get; set; } = EstadoTurno.Pendiente; // ahora se maneja con State

        public decimal Precio { get; set; }

        public Valoracion? Valoracion { get; set; }
        public bool FueValorado { get; set; } = false;

        public DateTime HoraInicio => FechaHora;
        public DateTime HoraFin => FechaHora.Add(Duracion);

        // ======== NUEVO: Patrón State ========
        public IEstadoTurno EstadoActual => ObtenerEstado();

        private IEstadoTurno ObtenerEstado()
        {
            return Estado switch
            {
                EstadoTurno.Pendiente => new EstadoPendiente(),
                EstadoTurno.PendientePago => new EstadoPendientePago(),
                EstadoTurno.Confirmado => new EstadoConfirmado(),
                EstadoTurno.Completado => new EstadoCompletado(),
                EstadoTurno.Cancelado => new EstadoCancelado(),
                _ => new EstadoPendiente()
            };
        }
        // =====================================
    }
}
