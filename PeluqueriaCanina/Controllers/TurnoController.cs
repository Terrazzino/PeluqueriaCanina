using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDePeluquero;
using PeluqueriaCanina.Models.ClasesDeTurno;
using PeluqueriaCanina.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeluqueriaCanina.Controllers
{
    public class TurnoController : Controller
    {
        private readonly ContextoAcqua _contexto;
        private readonly IUsuarioActualService _usuarioActual;

        public TurnoController(ContextoAcqua contexto, IUsuarioActualService usuarioActual)
        {
            _contexto = contexto;
            _usuarioActual = usuarioActual;
        }


        // GET: Turno/Crear
        [HttpGet]
        [PermisoRequerido("RegistrarTurno")]
        public IActionResult Crear()
        {
            var clienteId = _usuarioActual.Obtener().Id;

            ViewBag.Mascotas = _contexto.Mascotas.Where(t=>t.ClienteId==clienteId).ToList();
            ViewBag.Servicios = _contexto.Servicios.ToList();
            ViewBag.Peluqueros = _contexto.Peluqueros.ToList();
            return View(new Turno());
        }

        // POST: Turno/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermisoRequerido("RegistrarTurno")]
        public async Task<IActionResult> Crear(Turno turno)
        {
            var errores = new List<string>();

            if (turno.MascotaId <= 0 || turno.ServicioId <= 0 || turno.FechaHora == default)
                errores.Add("Debe seleccionar una mascota, servicio y horario válido.");

            // 1️⃣ Obtener entidades de BD y marcar como Unchanged para no intentar insertarlas
            turno.Mascota = await _contexto.Mascotas.FindAsync(turno.MascotaId);
            if (turno.Mascota == null)
                errores.Add("La mascota seleccionada no existe.");
            else
                _contexto.Entry(turno.Mascota).State = EntityState.Unchanged;

            turno.Servicio = await _contexto.Servicios.FindAsync(turno.ServicioId);
            if (turno.Servicio == null)
                errores.Add("El servicio seleccionado no existe.");

            if (turno.PeluqueroId.HasValue)
            {
                turno.Peluquero = await _contexto.Peluqueros.FindAsync(turno.PeluqueroId.Value);
                if (turno.Peluquero != null)
                    _contexto.Entry(turno.Peluquero).State = EntityState.Unchanged;
            }

            // 2️⃣ Validar si la mascota tiene turnos activos (solo Pendiente o Confirmado)
            bool mascotaOcupada = await _contexto.Turnos
                .AnyAsync(t => t.MascotaId == turno.MascotaId &&
                               (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.Confirmado));

            if (mascotaOcupada)
                errores.Add("La mascota ya tiene un turno activo. No puede reservar otro.");

            // 3️⃣ Buscar peluquero disponible si no se eligió
            if (!turno.PeluqueroId.HasValue && turno.Servicio != null)
            {
                var peluqueroDisponible = await BuscarPeluqueroDisponible(turno.FechaHora, turno.Servicio.Duracion);
                if (peluqueroDisponible == null)
                    errores.Add("No hay peluqueros disponibles en ese horario.");
                else
                    turno.PeluqueroId = peluqueroDisponible.Value;
            }

            // 4️⃣ Si hay errores, devolverlos como JSON
            if (errores.Any())
                return Json(new { ok = false, message = string.Join("\n", errores) });

            // 5️⃣ Completar datos del turno
            turno.Estado = EstadoTurno.Pendiente;
            turno.Duracion = turno.Servicio.Duracion;
            turno.FechaHora = turno.FechaHora.ToLocalTime();
            turno.Precio = turno.Servicio.Precio;

            // 6️⃣ Guardar el turno
            _contexto.Turnos.Add(turno);
            await _contexto.SaveChangesAsync();

            return Json(new
            {
                ok = true,
                message = "Turno creado exitosamente. Procede al pago para confirmar.",
                turnoId = turno.Id  // ← IMPORTANTE: Agregar esto
            });
        }




        [HttpPost]
        [PermisoRequerido("CancelarTurno")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var turno = await _contexto.Turnos.FindAsync(id);
            if (turno == null)
                return Json(new { ok = false, message = "El turno no existe."});
            if (turno.Estado == EstadoTurno.Cancelado)
                return Json(new { ok = false, message = "El turno ya esta cancelado."});
            turno.Estado = EstadoTurno.Cancelado;
            await _contexto.SaveChangesAsync();
            return Json(new { ok = true, message = "Turno cancelado correctamente" });
        }

        // POST: Turno/Modificar
        // ✅ VERSIÓN FINAL CON FIX DE ZONA HORARIA
        [HttpPost]
        [PermisoRequerido("ModificarTurno")]
        public async Task<IActionResult> Modificar(int id, DateTime fechaHora)
        {
            try
            {
                // ✅ FIX ZONA HORARIA: Convertir a hora local si viene en UTC
                if (fechaHora.Kind == DateTimeKind.Utc)
                {
                    fechaHora = fechaHora.ToLocalTime();
                }
                // Si es Unspecified, asumir que ya está en hora local argentina
                else if (fechaHora.Kind == DateTimeKind.Unspecified)
                {
                    fechaHora = DateTime.SpecifyKind(fechaHora, DateTimeKind.Local);
                }

                // ✅ Validación de fecha en el pasado
                if (fechaHora < DateTime.Now)
                {
                    return BadRequest(new { ok = false, message = "No podés seleccionar un horario en el pasado." });
                }

                var turno = await _contexto.Turnos
                    .Include(t => t.Servicio)
                    .Include(t => t.Peluquero)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (turno == null)
                {
                    return NotFound(new { ok = false, message = "El turno no existe." });
                }

                // ✅ Validación de estado cancelado
                if (turno.Estado == EstadoTurno.Cancelado)
                {
                    return BadRequest(new { ok = false, message = "No se puede modificar un turno cancelado." });
                }

                // ✅ Duración real del turno
                var duracionTurno = turno.Duracion != TimeSpan.Zero
                    ? turno.Duracion
                    : turno.Servicio?.Duracion ?? TimeSpan.FromMinutes(60);

                // ✅ Cargar turnos del peluquero para validar solapamiento
                var turnosDelPeluquero = await _contexto.Turnos
                    .Include(t => t.Servicio)
                    .Where(t =>
                        t.PeluqueroId == turno.PeluqueroId &&
                        t.Id != id &&
                        t.Estado != EstadoTurno.Cancelado)
                    .ToListAsync();

                // ✅ Validar solapamiento en memoria
                bool solapa = turnosDelPeluquero.Any(t =>
                {
                    var duracionExistente = t.Duracion != TimeSpan.Zero
                        ? t.Duracion
                        : (t.Servicio?.Duracion ?? TimeSpan.FromMinutes(60));

                    var finExistente = t.FechaHora.Add(duracionExistente);
                    var finNuevo = fechaHora.Add(duracionTurno);

                    return fechaHora < finExistente && t.FechaHora < finNuevo;
                });

                if (solapa)
                {
                    return BadRequest(new { ok = false, message = "El turno se superpone con otro existente del mismo peluquero." });
                }

                // ✅ Validar día de la semana
                var diaSemana = MapearDia(fechaHora.DayOfWeek);
                if (diaSemana == null)
                {
                    return BadRequest(new { ok = false, message = "No hay atención ese día de la semana." });
                }

                // ✅ Validar horario laboral
                var jornadaDelDia = await _contexto.Jornadas
                    .FirstOrDefaultAsync(j =>
                        j.PeluqueroId == turno.PeluqueroId &&
                        j.Dia == diaSemana &&
                        j.Activo);

                if (jornadaDelDia == null)
                {
                    return BadRequest(new { ok = false, message = "El peluquero no trabaja ese día." });
                }

                var horaInicio = fechaHora.TimeOfDay;
                var horaFin = horaInicio.Add(duracionTurno);

                if (horaInicio < jornadaDelDia.HoraDeInicio || horaFin > jornadaDelDia.HoraDeFin)
                {
                    return BadRequest(new { ok = false, message = "El horario seleccionado está fuera del horario laboral." });
                }

                // ✅ Actualizar el turno
                turno.FechaHora = fechaHora;

                _contexto.Turnos.Update(turno);
                await _contexto.SaveChangesAsync();

                return Ok(new { ok = true, message = "Turno modificado con éxito." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Modificar turno {id}: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    ok = false,
                    message = "Ocurrió un error al modificar el turno. Por favor, intentá más tarde.",
                    error = ex.Message
                });
            }
        }





        [HttpGet]
        [PermisoRequerido("VerDisponibilidadTurnos")]
        public async Task<IActionResult> GetDisponibilidad(int? servicioId, int? peluqueroId, DateTime? start, DateTime? end, int? duracion)
        {
            if (!start.HasValue || !end.HasValue)
                return Json(new List<object>());

            var servicio = servicioId.HasValue ? await _contexto.Servicios.FindAsync(servicioId) : null;

            int duracionMinutos = duracion ??
                (servicio != null && servicio.Duracion.TotalMinutes > 0
                    ? (int)servicio.Duracion.TotalMinutes
                    : 30);

            var turnosOcupados = await _contexto.Turnos
                .Where(t =>
                    (peluqueroId != null ? t.PeluqueroId == peluqueroId : true) &&
                    t.Estado != EstadoTurno.Cancelado)
                .ToListAsync();

            var jornadas = await _contexto.Jornadas
                .Where(j => (peluqueroId != null ? j.PeluqueroId == peluqueroId : true) && j.Activo)
                .ToListAsync();

            var eventos = new List<object>();

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

                        if (fin > fecha + horaFin)
                            break;

                        bool ocupado = turnosOcupados.Any(t =>
                        {
                            var turnoFin = t.FechaHora.Add(t.Duracion);
                            return inicio < turnoFin && fin > t.FechaHora;
                        });

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
                    }
                }
            }

            return Json(eventos);
        }





        // GET: Turno
        [PermisoRequerido("RegistrarTurno")]
        public async Task<IActionResult> Index()
        {
            var clienteId = _usuarioActual.Obtener().Id;
            var turnos = await _contexto.Turnos
                .Include(t => t.Mascota).Where(t=>t.Mascota.ClienteId==clienteId)
                .Include(t => t.Servicio)
                .Include(t => t.Peluquero)
                .Include(t => t.Valoracion)
                .Where(t=>t.Estado!=EstadoTurno.Cancelado)
                .ToListAsync();
            return View(turnos);
        }

        [PermisoRequerido("RegistrarTurno")]
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

        [PermisoRequerido("RegistrarTurno")]
        private async Task<int?> BuscarPeluqueroDisponible(DateTime fechaHora, TimeSpan duracion)
        {
            var peluqueros = await _contexto.Peluqueros.Include(p => p.Turnos).ToListAsync();

            foreach (var p in peluqueros)
            {
                bool ocupado = p.Turnos.Any(t =>
                    t.Estado != EstadoTurno.Cancelado &&
                    fechaHora < t.FechaHora.Add(t.Duracion) &&
                    t.FechaHora < fechaHora.Add(duracion));

                if (!ocupado)
                    return p.Id; // devuelve el primero disponible
            }

            return null; // ninguno libre
        }


    }
}
