using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDePeluquero;
using PeluqueriaCanina.Models.ClasesDeTurno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeluqueriaCanina.Controllers
{
    public class TurnoController : Controller
    {
        private readonly ContextoAcqua _contexto;

        public TurnoController(ContextoAcqua contexto)
        {
            _contexto = contexto;
        }

        // GET: Turno/Crear
        public IActionResult Crear()
        {
            ViewBag.Mascotas = _contexto.Mascotas.ToList();
            ViewBag.Servicios = _contexto.Servicios.ToList();
            ViewBag.Peluqueros = _contexto.Peluqueros.ToList();
            return View();
        }

        // POST: Turno/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Turno turno)
        {
            if (ModelState.IsValid)
            {
                _contexto.Add(turno);
                await _contexto.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Mascotas = _contexto.Mascotas.ToList();
            ViewBag.Servicios = _contexto.Servicios.ToList();
            ViewBag.Peluqueros = _contexto.Peluqueros.ToList();
            return View(turno);
        }

        // 🔥 Unificado: disponibilidad dinámica para FullCalendar
        [HttpGet]
        public async Task<IActionResult> GetDisponibilidad(int? servicioId, int? peluqueroId, DateTime? start, DateTime? end)
        {
            if (!start.HasValue || !end.HasValue)
                return Json(new List<object>());

            var servicio = servicioId.HasValue ? await _contexto.Servicios.FindAsync(servicioId) : null;
            int duracion = servicio?.Duracion.Minutes ?? 30;

            // Traemos turnos ocupados del peluquero (todos los servicios)
            var turnosOcupados = await _contexto.Turnos
                .Where(t => (peluqueroId != null ? t.PeluqueroId == peluqueroId : true)
                            && t.Estado != EstadoTurno.Cancelled
                            && t.FechaHora >= start && t.FechaHora < end)
                .ToListAsync();

            var eventos = new List<object>();

            List<Jornada> jornadas = new();
            if (peluqueroId != null)
            {
                jornadas = await _contexto.Jornadas
                    .Where(j => j.PeluqueroId == peluqueroId && j.Activo)
                    .ToListAsync();
            }
            else
            {
                jornadas = await _contexto.Jornadas
                    .Where(j => j.Activo)
                    .ToListAsync();
            }
            for (DateTime fecha = start.Value.Date; fecha <= end.Value.Date; fecha = fecha.AddDays(1))
            {
                var dia = MapearDia(fecha.DayOfWeek);
                if (dia == null) continue;

                var jornadasDelDia = jornadas.Where(j => j.Dia == dia).ToList();
                if (!jornadasDelDia.Any()) continue;

                foreach (var jornada in jornadasDelDia)
                {
                    var horaInicio = jornada.HoraDeInicio;
                    var horaFin = jornada.HoraDeFin;

                    for (var hora = horaInicio; hora < horaFin; hora = hora.Add(TimeSpan.FromMinutes(duracion)))
                    {
                        var inicio = fecha + hora;
                        var fin = inicio.AddMinutes(duracion);

                        bool ocupado = turnosOcupados.Any(t =>
                        (inicio < t.FechaHora.AddMinutes(t.Duracion.Minutes)) &&
                        (t.FechaHora < fin));

                        eventos.Add(new
                        {
                            title = ocupado ? "Ocupado":"Disponible",
                            start = inicio.ToString("s"),
                            end = fin.ToString("s"),
                            allDay=false,
                            backgroundColor = ocupado ? "#dc3545":"#28a745",
                            editable = false
                        });
                    }
                }
            }
            return Json(eventos);
        }

        // GET: Turno
        public async Task<IActionResult> Index()
        {
            var turnos = await _contexto.Turnos
                .Include(t => t.Mascota)
                .Include(t => t.Servicio)
                .Include(t => t.Peluquero)
                .ToListAsync();
            return View(turnos);
        }

        private DiasLaborales? MapearDia(DayOfWeek dow)
        {
            return dow switch
            {
                DayOfWeek.Monday=>DiasLaborales.Lunes,
                DayOfWeek.Tuesday=> DiasLaborales.Martes,
                DayOfWeek.Wednesday=>DiasLaborales.Miercoles,
                DayOfWeek.Thursday=>DiasLaborales.Jueves,
                DayOfWeek.Friday=>DiasLaborales.Viernes,
                DayOfWeek.Saturday=> DiasLaborales.Sabado,
                _ => null
            };
        }

    }
}
