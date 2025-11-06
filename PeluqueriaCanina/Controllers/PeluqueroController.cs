using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDeTurno;
using System.Linq;

namespace PeluqueriaCanina.Controllers
{
    public class PeluqueroController : Controller
    {
        private readonly ContextoAcqua _context;

        public PeluqueroController(ContextoAcqua context)
        {
            _context = context;
        }

        // ✅ Muestra el calendario
        public IActionResult Agenda()
        {
            return View();
        }

        // ✅ Retorna turnos en formato JSON para el calendario
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
        public IActionResult CambiarEstado(int turnoId, EstadoTurno nuevoEstado)
        {
            var turno = _context.Turnos.Find(turnoId);
            if (turno == null) return NotFound();

            turno.Estado = nuevoEstado;
            _context.SaveChanges();

            return Json(new { ok = true });
        }
    }
}
