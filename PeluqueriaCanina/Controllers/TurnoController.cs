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
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Mascotas = _contexto.Mascotas.ToList();
            ViewBag.Servicios = _contexto.Servicios.ToList();
            ViewBag.Peluqueros = _contexto.Peluqueros.ToList();
            return View(new Turno());
        }

        // POST: Turno/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Turno turno)
        {
            if (turno.MascotaId <= 0 || turno.ServicioId <= 0 || turno.FechaHora == default)
            {
                ModelState.AddModelError("", "Debe seleccionar una mascota, servicio y horario válido.");
                ViewBag.Mascotas = _contexto.Mascotas.ToList();
                ViewBag.Servicios = _contexto.Servicios.ToList();
                ViewBag.Peluqueros = _contexto.Peluqueros.ToList();
                return View(turno);
            }
            //Asignar las entidades completas desde la BD
            turno.Mascota = await _contexto.Mascotas.FindAsync(turno.MascotaId);
            turno.Servicio = await _contexto.Servicios.FindAsync(turno.ServicioId);
            turno.Peluquero = turno.PeluqueroId.HasValue
                ? await _contexto.Peluqueros.FindAsync(turno.PeluqueroId.Value)
                : null;

            // 1️.Validar datos básicos
            if (turno.Mascota == null || turno.Servicio == null || turno.FechaHora == default)
            {
                ModelState.AddModelError("", "Debe seleccionar una mascota, servicio y horario válido.");
            }

            // 2️.Validar que la mascota no tenga turnos activos
            bool mascotaOcupada = await _contexto.Turnos
                .AnyAsync(t => t.MascotaId == turno.MascotaId &&
                               (t.Estado == EstadoTurno.PendingPayment || t.Estado == EstadoTurno.Confirmed));
            if (mascotaOcupada)
            {
                ModelState.AddModelError("", "Esta mascota ya tiene un turno activo. Debe finalizarlo o cancelarlo antes de reservar otro.");
            }

            // 3️.Si el servicio no existe (por seguridad extra)
            if (turno.Servicio == null)
            {
                ModelState.AddModelError("", "El servicio seleccionado no existe.");
            }

            // 4️.Buscar peluquero disponible si no se eligió
            if (turno.PeluqueroId == null)
            {
                var peluqueroDisponible = await BuscarPeluqueroDisponible(turno.FechaHora, turno.Servicio.Duracion);
                if (peluqueroDisponible == null)
                {
                    ModelState.AddModelError("", "No hay peluqueros disponibles en ese horario.");
                    ViewBag.Mascotas = _contexto.Mascotas.ToList();
                    ViewBag.Servicios = _contexto.Servicios.ToList();
                    ViewBag.Peluqueros = _contexto.Peluqueros.ToList();
                    return View(turno);
                }

                turno.PeluqueroId = peluqueroDisponible.Value;
            }

            // 5️.Completar datos del turno
            turno.Estado = EstadoTurno.PendingPayment;
            turno.Duracion = turno.Servicio.Duracion;

            // 6️.Guardar
            _contexto.Turnos.Add(turno);
            await _contexto.SaveChangesAsync();

            TempData["Success"] = "Turno reservado correctamente.";
            return RedirectToAction(nameof(Index));
        }



        //Unificado: disponibilidad dinámica para FullCalendar
        [HttpGet]
        public async Task<IActionResult> GetDisponibilidad(int? servicioId, int? peluqueroId, DateTime? start, DateTime? end)
        {
            if (!start.HasValue || !end.HasValue)
                return Json(new List<object>());

            //Duración del servicio seleccionado
            var servicio = servicioId.HasValue ? await _contexto.Servicios.FindAsync(servicioId) : null;
            int duracionMinutos = servicio != null && servicio.Duracion.TotalMinutes > 0
                ? (int)servicio.Duracion.TotalMinutes
                : 30;

            //Turnos ocupados dentro del rango visible
            var turnosOcupados = await _contexto.Turnos
                .Where(t =>
                    (peluqueroId != null ? t.PeluqueroId == peluqueroId : true) &&
                    t.Estado != EstadoTurno.Cancelled &&
                    t.FechaHora >= start && t.FechaHora < end)
                .ToListAsync();

            var eventos = new List<object>();

            //Jornadas activas (del peluquero o de todos)
            var jornadas = await _contexto.Jornadas
                .Where(j => (peluqueroId != null ? j.PeluqueroId == peluqueroId : true) && j.Activo)
                .ToListAsync();

            //Recorremos los días visibles
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

        private async Task<int?> BuscarPeluqueroDisponible(DateTime fechaHora, TimeSpan duracion)
        {
            var peluqueros = await _contexto.Peluqueros.Include(p => p.Turnos).ToListAsync();

            foreach (var p in peluqueros)
            {
                bool ocupado = p.Turnos.Any(t =>
                    t.Estado != EstadoTurno.Cancelled &&
                    fechaHora < t.FechaHora.Add(t.Duracion) &&
                    t.FechaHora < fechaHora.Add(duracion));

                if (!ocupado)
                    return p.Id; // devuelve el primero disponible
            }

            return null; // ninguno libre
        }


    }
}
