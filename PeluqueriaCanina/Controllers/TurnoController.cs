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

            // 🔹 Duración del servicio seleccionado
            var servicio = servicioId.HasValue ? await _contexto.Servicios.FindAsync(servicioId) : null;
            int duracionMinutos = servicio != null && servicio.Duracion.TotalMinutes > 0
                ? (int)servicio.Duracion.TotalMinutes
                : 30;

            // 🔹 Turnos ocupados dentro del rango visible
            var turnosOcupados = await _contexto.Turnos
                .Where(t =>
                    (peluqueroId != null ? t.PeluqueroId == peluqueroId : true) &&
                    t.Estado != EstadoTurno.Cancelled &&
                    t.FechaHora >= start && t.FechaHora < end)
                .ToListAsync();

            var eventos = new List<object>();

            // 🔹 Jornadas activas (del peluquero o de todos)
            var jornadas = await _contexto.Jornadas
                .Where(j => (peluqueroId != null ? j.PeluqueroId == peluqueroId : true) && j.Activo)
                .ToListAsync();

            // 🔹 Recorremos los días visibles
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

                    for (var hora = horaInicio; hora < horaFin; hora = hora.Add(TimeSpan.FromMinutes(duracionMinutos)))
                    {
                        var inicio = fecha + hora;
                        var fin = inicio.AddMinutes(duracionMinutos);

                        // ❌ Si el turno se pasa del horario laboral, no se ofrece
                        if (fin > fecha + horaFin)
                            break;

                        // ❌ Si se solapa con un turno existente, no se ofrece
                        bool ocupado = turnosOcupados.Any(t =>
                            (inicio < t.FechaHora.AddMinutes(t.Duracion.Minutes)) &&
                            (t.FechaHora < fin));

                        // ✅ Solo agregamos si hay disponibilidad real
                        if (!ocupado)
                        {
                            eventos.Add(new
                            {
                                title = "Disponible",
                                start = inicio.ToString("s"),
                                end = fin.ToString("s"),
                                allDay = false,
                                backgroundColor = "#28a745",
                                borderColor = "#28a745",
                                editable = false
                            });
                        }
                        else
                        {
                            eventos.Add(new
                            {
                                title = "Ocupado",
                                start = inicio.ToString("s"),
                                end = fin.ToString("s"),
                                allDay = false,
                                backgroundColor = "#dc3545",
                                borderColor = "#dc3545",
                                editable = false
                            });
                        }
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
