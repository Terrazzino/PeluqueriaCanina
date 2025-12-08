using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDePeluquero;
using PeluqueriaCanina.Models.Factories;
using PeluqueriaCanina.Models.Users;
using PeluqueriaCanina.Models.VMs;
using PeluqueriaCanina.Services;
using System.Security.Cryptography;
using System.Text;

namespace PeluqueriaCanina.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly ContextoAcqua _contexto;
        private readonly IEmailSender _emailSender;
        private readonly IUsuarioActualService _usuarioActual;

        public AdministradorController(ContextoAcqua context, IEmailSender emailSender, IUsuarioActualService usuarioActual)
        {
            _contexto = context;
            _emailSender = emailSender;
            _usuarioActual = usuarioActual;
        }

        [PermisoRequerido("AccederDashboardAdministrador")]
        public IActionResult Dashboard() => View();

        [HttpGet]
        [PermisoRequerido("RegistrarPeluquero")]
        public IActionResult RegistrarPeluquero() => View();

        [HttpPost]
        [PermisoRequerido("RegistrarPeluquero")]
        public async Task<IActionResult> RegistrarPeluquero(
    string nombre,
    string apellido,
    string mail,
    string dni,
    DateTime fechaDeNacimiento,
    List<Jornada> jornadas)
        {
            if (_contexto.Personas.Any(p => p.Mail == mail))
            {
                ModelState.AddModelError("", "El mail ya se encuentra registrado");
                return View();
            }

            var contraseñaRandom = GenerarContraseña();

            var nuevoPeluquero = UsuarioFactory.CrearUsuario<Peluquero>(
                nombre,
                apellido,
                mail,
                dni,
                fechaDeNacimiento,
                contraseñaRandom
            );

            // ASIGNAR JORNADAS
            nuevoPeluquero.Jornadas = jornadas
                .Where(j => j.HoraDeInicio != TimeSpan.Zero && j.HoraDeFin != TimeSpan.Zero && j.Activo)
                .ToList();

            // BUSCAR GRUPO "Peluquero"
            var grupoPeluquero = _contexto.Grupos.FirstOrDefault(g => g.Nombre == "Peluquero");

            if (grupoPeluquero == null)
            {
                ModelState.AddModelError("", "No existe el grupo 'Peluquero'. Debes crearlo antes.");
                return View();
            }

            // ASIGNAR GRUPO AL NUEVO PELUQUERO
            nuevoPeluquero.Grupos.Add(grupoPeluquero);

            // GUARDAR
            _contexto.Personas.Add(nuevoPeluquero);
            await _contexto.SaveChangesAsync();

            // ENVIAR MAIL
            await EnviarMailConContraseña(mail, contraseñaRandom, nombre);

            return RedirectToAction("Dashboard");
        }


        private string GenerarContraseña()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var data = new byte[8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(data);
            var result = new char[8];
            for (int i = 0; i < result.Length; i++)
                result[i] = chars[data[i] % chars.Length];
            return new string(result);
        }

        private async Task EnviarMailConContraseña(string mail, string contraseña, string nombre)
        {
            try
            {
                await _emailSender.SendEmailAsync(
                    mail,
                    "Bienvenido a AcquaDiCane",
                    $"Hola {nombre}!<br/>Tu contraseña temporal es: <b>{contraseña}</b><br/>Por favor, cámbiala luego de iniciar sesión."
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar mail: {ex.Message}");
            }
        }

        [PermisoRequerido("VerPeluqueros")]
        public IActionResult ListarPeluqueros()
        {
            var peluqueros = _contexto.Personas
                .OfType<Peluquero>()
                .Include(p => p.Jornadas)
                .ToList();
            return View(peluqueros);
        }

        [HttpGet]
        [PermisoRequerido("ModificarPeluquero")]
        public IActionResult EditarPeluquero(int id)
        {
            var peluquero = _contexto.Personas
                .OfType<Peluquero>()
                .Include(p => p.Jornadas)
                .FirstOrDefault(p => p.Id == id);

            if (peluquero == null) return NotFound();

            foreach (DiasLaborales dia in Enum.GetValues(typeof(DiasLaborales)))
            {
                if (!peluquero.Jornadas.Any(j => j.Dia == dia))
                {
                    peluquero.Jornadas.Add(new Jornada
                    {
                        Dia = dia,
                        Activo = false,
                        HoraDeInicio = TimeSpan.Zero,
                        HoraDeFin = TimeSpan.Zero
                    });
                }
            }

            peluquero.Jornadas = peluquero.Jornadas.OrderBy(j => j.Dia).ToList();
            return View(peluquero);
        }

        [HttpPost]
        [PermisoRequerido("ModificarPeluquero")]
        public async Task<IActionResult> EditarPeluquero(Peluquero peluqueroActualizado)
        {
            var peluquero = _contexto.Personas
                .OfType<Peluquero>()
                .Include(p => p.Jornadas)
                .FirstOrDefault(p => p.Id == peluqueroActualizado.Id);

            if (peluquero == null) return NotFound();

            peluquero.Nombre = peluqueroActualizado.Nombre;
            peluquero.Apellido = peluqueroActualizado.Apellido;
            peluquero.Mail = peluqueroActualizado.Mail;

            foreach (var jornadaEditada in peluqueroActualizado.Jornadas)
            {
                var jornada = peluquero.Jornadas.FirstOrDefault(j => j.Id == jornadaEditada.Id && j.Id != 0);
                if (jornada != null)
                {
                    jornada.Activo = jornadaEditada.Activo;
                    jornada.HoraDeInicio = jornadaEditada.HoraDeInicio;
                    jornada.HoraDeFin = jornadaEditada.HoraDeFin;
                }
                else
                {
                    peluquero.Jornadas.Add(new Jornada
                    {
                        Dia = jornadaEditada.Dia,
                        Activo = jornadaEditada.Activo,
                        HoraDeInicio = jornadaEditada.HoraDeInicio,
                        HoraDeFin = jornadaEditada.HoraDeFin
                    });
                }
            }

            _contexto.Update(peluquero);
            await _contexto.SaveChangesAsync();
            return RedirectToAction("ListarPeluqueros");
        }

        [PermisoRequerido("EliminarPeluquero")]
        public async Task<IActionResult> EliminarPeluquero(int id)
        {
            var peluquero = _contexto.Personas.OfType<Peluquero>().FirstOrDefault(p => p.Id == id);
            if (peluquero == null) return NotFound();

            _contexto.Remove(peluquero);
            await _contexto.SaveChangesAsync();
            return RedirectToAction("ListarPeluqueros");
        }

        // === REPORTE PRINCIPAL ===
        [PermisoRequerido("VerReporte")]
        public IActionResult Reportes()
        {
            // Consulta la vista en BD
            var datos = _contexto.vw_ReporteServiciosTotales
                .OrderByDescending(x => x.Total)
                .ToList();

            ViewBag.Servicios = datos.Select(d => d.NombreServicio).ToList();
            ViewBag.Cantidades = datos.Select(d => d.Total).ToList();

            return View();
        }

        // === DETALLE DE SERVICIO ===
        [PermisoRequerido("VerReporte")]
        public IActionResult DetalleServicio(string servicio)
        {
            if (string.IsNullOrEmpty(servicio))
                return RedirectToAction("Reportes");

            // 🔥 IMPORTANTE: Decodificar
            servicio = System.Web.HttpUtility.UrlDecode(servicio);
            servicio = System.Web.HttpUtility.HtmlDecode(servicio);

            var datos = _contexto.vw_ReportePeluquerosPorServicio
                .Where(p => p.NombreServicio == servicio)
                .ToList();

            ViewBag.Servicio = servicio;
            ViewBag.Peluqueros = datos.Select(d => d.NombrePeluquero).ToList();
            ViewBag.Cantidades = datos.Select(d => d.Cantidad).ToList();

            return View();
        }


        // === DETALLE DE PELUQUERO ===
        [PermisoRequerido("VerReporte")]
        public IActionResult DetallePeluquero(string servicio, string peluquero)
        {
            if (string.IsNullOrEmpty(servicio) || string.IsNullOrEmpty(peluquero))
                return RedirectToAction("Reportes");

            // 🔥 Decodificar ambos valores
            servicio = System.Web.HttpUtility.UrlDecode(servicio);
            servicio = System.Web.HttpUtility.HtmlDecode(servicio);

            peluquero = System.Web.HttpUtility.UrlDecode(peluquero);
            peluquero = System.Web.HttpUtility.HtmlDecode(peluquero);

            var detalle = _contexto.vw_ReporteDetallePeluquero
                .AsEnumerable()
                .FirstOrDefault(r =>
                    Normalizar(r.NombreServicio) == Normalizar(servicio) &&
                    Normalizar(r.NombrePeluquero) == Normalizar(peluquero));

            if (detalle == null)
            {
                ViewBag.Peluquero = peluquero;
                ViewBag.Servicio = servicio;
                ViewBag.Recaudado = 0m;
                ViewBag.Realizados = 0;
                ViewBag.Cancelados = 0;
                return View();
            }

            ViewBag.Peluquero = detalle.NombrePeluquero;
            ViewBag.Servicio = detalle.NombreServicio;
            ViewBag.Recaudado = detalle.Recaudado;
            ViewBag.Realizados = detalle.Realizados;
            ViewBag.Cancelados = detalle.Cancelados;

            return View();
        }


        private string Normalizar(string texto)
        {
            if (texto == null) return "";
            return texto
                .Normalize(NormalizationForm.FormKC)
                .Trim()
                .ToLower();
        }

        [PermisoRequerido("VerValoracionDePeluqueros")]
        public IActionResult RankingPeluqueros()
        {
            var data = _contexto.Peluqueros
                .Select(p => new PeluqueroRankingVM
                {
                    PeluqueroId = p.Id,
                    NombreCompleto = p.Nombre + " " + p.Apellido,
                    Promedio = _contexto.Valoraciones
                                .Where(v => v.PeluqueroId == p.Id)
                                .Select(v => (double?)v.Puntuacion)
                                .DefaultIfEmpty(0)
                                .Average() ?? 0,
                    Cantidad = _contexto.Valoraciones.Count(v => v.PeluqueroId == p.Id)
                })
                .OrderByDescending(x => x.Promedio)
                .ToList();

            return View(data);
        }
    }
}
