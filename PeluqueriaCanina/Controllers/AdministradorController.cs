using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Models.ClasesDePeluquero;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.Users;
using PeluqueriaCanina.Models.Factories;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace PeluqueriaCanina.Controllers
{
    public class AdministradorController:Controller
    {
        private readonly ContextoAcqua _contexto;
        private readonly IEmailSender _emailSender;

        public AdministradorController(ContextoAcqua context, IEmailSender emailSender)
        {
            _contexto = context;
            _emailSender = emailSender;
        }
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Rol")!="Administrador")
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        [HttpGet]
        public IActionResult RegistrarPeluquero()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarPeluquero(string nombre, string apellido, string mail, string dni, DateTime fechaDeNacimiento, List<Jornada> jornadas)
        {
            if (_contexto.Personas.Any(p => p.Mail == mail))
            {
                ViewBag.Error = "El mail ya se encuentra registrado";
                return View();
            }

            var contraseñaRandom = GenerarContraseña();
            var nuevoPeluquero = UsuarioFactory.CrearUsuario<Peluquero>(
                nombre,
                apellido,
                mail,
                dni,
                fechaDeNacimiento,
                contraseñaRandom,
                "Peluquero"
            );

            nuevoPeluquero.Jornadas = jornadas
                .Where(j => j.HoraDeInicio != TimeSpan.Zero && j.HoraDeFin != TimeSpan.Zero && j.Activo)
                .ToList();

            _contexto.Personas.Add(nuevoPeluquero);
            _contexto.SaveChanges();

            await EnviarMailConContraseña(mail, contraseñaRandom, nombre);

            return RedirectToAction("Dashboard");
        }

        private string GenerarContraseña()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
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
                // También podrías guardarlo en un log o base de datos
            }
        }

        public IActionResult ListarPeluqueros()
        {
            if (HttpContext.Session.GetString("Rol")!="Administrador")
            {
                return RedirectToAction("Login","Auth");
            }
            var peluqueros = _contexto.Personas
                .OfType<Peluquero>()
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.Apellido,
                    p.Mail,
                    Jornadas = p.Jornadas.Select(j => new
                    {
                        j.Dia,
                        j.HoraDeInicio,
                        j.HoraDeFin,
                        j.Activo
                    })
                })
                .ToList();
            return View(peluqueros);
        }

        [HttpGet]
        public IActionResult EditarPeluquero(int id)
        {
            var peluquero = _contexto.Personas
                .OfType<Peluquero>()
                .Include(p=>p.Jornadas)
                .FirstOrDefault(p => p.Id == id);
            if (peluquero == null) return NotFound();
            var dias = Enum.GetValues(typeof(DiasLaborales)).Cast<DiasLaborales>();
            foreach(var dia in dias)
            {
                if (!peluquero.Jornadas.Any(j=>j.Dia==dia))
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
            peluquero.Jornadas = peluquero.Jornadas.OrderBy(j=>j.Dia).ToList();
            return View(peluquero);
        }

        [HttpPost]
        public IActionResult EditarPeluquero(Peluquero peluqueroActualizado)
        {
            var peluquero = _contexto.Personas
                .OfType<Peluquero>()
                .Include(p=>p.Jornadas)
                .FirstOrDefault(p => p.Id == peluqueroActualizado.Id);

            if (peluquero == null) return NotFound();

            peluquero.Nombre = peluqueroActualizado.Nombre;
            peluquero.Apellido = peluqueroActualizado.Apellido;
            peluquero.Mail = peluqueroActualizado.Mail;

            foreach (var jornadaEditada in peluqueroActualizado.Jornadas)
            {
                var jornada = peluquero.Jornadas.FirstOrDefault(j=>j.Id==jornadaEditada.Id && j.Id!=0);
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
            _contexto.SaveChanges();
            return RedirectToAction("ListarPeluqueros");
        }

        public IActionResult EliminarPeluquero(int id)
        {
            var peluquero = _contexto.Personas
                .OfType<Peluquero>()
                .FirstOrDefault(p => p.Id == id);

            if (peluquero == null) return NotFound();

            _contexto.Remove(peluquero);
            _contexto.SaveChanges();

            return RedirectToAction("ListarPeluqueros");
        }



        //DESARROLLAMOS EL HARDCODEO PARA BASE DE DATOS APLICADAS
        //EN ESTA SECCIÓN IMPLEMENTAMOS TODO LO REFERIDO A REPORTES
        //LUEGO IMPLEMENTAREMOS EL CONTEXTO PARA DARLE DINAMISMO
        //VAMOS A USAR LA LIBRERIA DE CHATR.JS PARA ESTE DESARROLLO


        public IActionResult Reportes()
        {
            var servicios = _contexto.ReporteServicios
                .Select(s => s.NombreServicio)
                .ToList();

            var cantidades = _contexto.ReporteServicios
                .Select(s => s.Cantidad)
                .ToList();

            ViewBag.Servicios = servicios;
            ViewBag.Cantidades = cantidades;

            return View();
        }

        public IActionResult DetalleServicio(string servicio)
        {
            var datos = _contexto.ReportePeluquerosPorServicio
                .Where(r => r.NombreServicio == servicio)
                .ToList();

            ViewBag.Servicio = servicio;
            ViewBag.Peluqueros = datos.Select(d => d.NombrePeluquero).ToList();
            ViewBag.Cantidades = datos.Select(d => d.Cantidad).ToList();

            return View();
        }

        public IActionResult DetallePeluquero(string servicio, string peluquero)
        {
            var datos = _contexto.ReporteDetallePeluquero
                .FirstOrDefault(r => r.NombrePeluquero == peluquero);

            if (datos == null)
            {
                ViewBag.Error = "No se encontraron datos del peluquero.";
                return View();
            }

            ViewBag.Servicio = servicio;
            ViewBag.Peluquero = peluquero;
            ViewBag.Realizados = datos.Realizados;
            ViewBag.Cancelados = datos.Cancelados;
            ViewBag.Recaudado = datos.Recaudado;

            return View();
        }


    }
}
