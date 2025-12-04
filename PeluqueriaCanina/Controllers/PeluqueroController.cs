using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDeTurno;
using PeluqueriaCanina.Services;
using System.Linq;

namespace PeluqueriaCanina.Controllers
{
    public class PeluqueroController : Controller
    {
        private readonly ContextoAcqua _context;
        private readonly IUsuarioActualService _usuarioActual;

        public PeluqueroController(ContextoAcqua context, IUsuarioActualService usuarioActual)
        {
            _context = context;
            _usuarioActual = usuarioActual;
        }

        // ✅ Muestra el calendario
        [PermisoRequerido("AccederDashboardPeluquero")]
        public IActionResult Agenda()
        {
            return View();
        }

        // ✅ Retorna turnos en formato JSON para el calendario
        [PermisoRequerido("VerTurnos")]
        public IActionResult GetTurnos(DateTime start, DateTime end)
        {
            var usuarioId = HttpContext.Session.GetString("UsuarioId");
            if (usuarioId == null) return Unauthorized();

            int peluqueroId = int.Parse(usuarioId);

            var turnos = _context.Turnos
                .Include(t => t.Mascota)
                .Include(t => t.Servicio)
                .Include(t => t.Mascota.Cliente)
                .Where(t => t.PeluqueroId == peluqueroId && t.FechaHora >= start && t.FechaHora <= end)
                .ToList() // ✅ IMPORTANTE: ahora EF ya no convierte lo de abajo a SQL
                .Select(t => new
                {
                    id = t.Id,
                    title = $"{t.Mascota.Nombre} - {t.Servicio.NombreDelServicio}",
                    start = t.FechaHora,
                    end = t.FechaHora.AddMinutes(t.Servicio.Duracion.TotalMinutes),
                    estado = t.Estado.ToString(),
                    backgroundColor = t.Estado switch
                    {
                        EstadoTurno.Pendiente => "#ffc107",
                        EstadoTurno.Confirmado => "#007bff",
                        EstadoTurno.Completado => "#28a745",
                        EstadoTurno.Cancelado => "#dc3545",
                        _ => "#6c757d"
                    }
                })
                .ToList();


            return Json(turnos);
        }

        // ✅ Cambia el estado de un turno
        [HttpPost]
        [PermisoRequerido("CambiarEstadoTurno")]
        public IActionResult CambiarEstado(int turnoId, EstadoTurno nuevoEstado)
        {
            var turno = _context.Turnos.Find(turnoId);
            if (turno == null) return NotFound();

            turno.Estado = nuevoEstado;
            _context.SaveChanges();

            return Json(new { ok = true });
        }

        // GET: Peluquero/MisValoraciones
        [PermisoRequerido("VerValoracionDePeluqueros")]
        public IActionResult MisValoraciones()
        {
            int peluqueroId = _usuarioActual.Obtener().Id;

            var valoraciones = _context.Valoraciones
                .Where(v => v.PeluqueroId == peluqueroId)
                .Include(v => v.Cliente)
                .ToList();

            return View(valoraciones);
        }

    }
}
