using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Services;
using System.Text.Json;

namespace PeluqueriaCanina.Controllers
{
    public class TurnoController:Controller
    {
        private readonly ContextoAcqua _contexto;
        public TurnoController(ContextoAcqua context)
        {
            _contexto = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Rol") != "Cliente")
                return RedirectToAction("Login", "Auth");

            var clienteId = int.Parse(HttpContext.Session.GetString("UsuarioId"));
            var turnos = _contexto.Turnos
                .Include(t => t.Mascota)
                .Include(t => t.Peluquero)
                .Where(t => t.Mascota.ClienteId == clienteId)
                .ToList();
            return View(turnos);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            if (HttpContext.Session.GetString("Rol") != "Cliente")
                return RedirectToAction("Login", "Auth");

            var clienteId = int.Parse(HttpContext.Session.GetString("UsuarioId"));

            var mascotas = _contexto.Mascotas
                .Where(m => m.ClienteId == clienteId)
                .ToList();

            var servicios = _contexto.Servicios.ToList();

            var peluqueros = _contexto.Peluqueros
                .Include(p=>p.Jornadas)
                .ToList();


            ViewBag.Mascotas = mascotas;
            ViewBag.Servicios = servicios;
            ViewBag.Peluqueros = peluqueros;

            var franjas = CalcularDisponibilidad(null, TimeSpan.FromMinutes(30),DateTime.Today, DateTime.Today.AddDays(7));

            var eventos = franjas.Select(f => new
            {
                title = f.Libre ? $"Libre - {f.PeluqueroNombre}":$"Ocupado - {f.PeluqueroNombre}",
                start = DateTime.Today.AddDays((int)f.Dia).Add(f.HoraInicio).ToString("s"),
                end = DateTime.Today.AddDays((int)f.Dia).Add(f.HoraFin).ToString("s"),
                color = f.Libre? "#d4edda" : "#f8d7da",
                borderColor = "#ccc",
            }).ToList();

            ViewBag.Eventos = eventos;

            return View();
        }

        private List<FranjaDisponible> CalcularDisponibilidad(int? peluqueroId, TimeSpan DuracionServicio, DateTime fechaDesde, DateTime fechaHasta)
        {
            var peluqueros = peluqueroId.HasValue
                ? _contexto.Peluqueros.Where(p => p.Id == peluqueroId.Value).Include(p => p.Jornadas).ToList()
                : _contexto.Peluqueros.Include(p => p.Jornadas).ToList();
            var franjasDisponibles = new List<FranjaDisponible>();

            foreach(var p in peluqueros)
            {
                foreach (var jornada in p.Jornadas)
                {
                    var turnosOcupados = _contexto.Turnos
                        .Where(t => t.PeluqueroId == p.Id &&
                        t.FechaHora.Date == fechaDesde.Date &&
                        t.Estado != EstadoTurno.Cancelled)
                        .ToList();

                    var inicio = jornada.HoraDeInicio;
                    var inicioDT = fechaDesde + inicio;
                    while(inicio + DuracionServicio <= jornada.HoraDeFin)
                    {
                        var fin = inicio + DuracionServicio;
                        var finDT = inicioDT + DuracionServicio;
                        var libre = !turnosOcupados.Any(t => t.HoraInicio<finDT && t.HoraFin > inicioDT);
                        franjasDisponibles.Add(new FranjaDisponible
                        {
                            PeluqueroId = p.Id,
                            PeluqueroNombre = $"{p.Nombre} {p.Apellido}",
                            Dia = jornada.Dia,
                            HoraInicio = inicio,
                            HoraFin = fin,
                            Libre = libre
                        });
                        inicio = inicio.Add(TimeSpan.FromMinutes(30));
                    }
                }
            }
            return franjasDisponibles;
        }
    }
}
