using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.Users;
using System.Security.Claims;

namespace PeluqueriaCanina.Controllers
{
    public class AuthController : Controller
    {
        private readonly ContextoAcqua _contexto;

        public AuthController(ContextoAcqua contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string mail, string contraseña)
        {
            // Buscar usuario en la tabla Personas
            var usuario = _contexto.Personas
                .Include(u => u.Grupos)
                .ThenInclude(g => g.Permisos)
                .FirstOrDefault(u => u.Mail == mail);

            // Validar existencia y contraseña
            if (usuario == null || !usuario.VerificarContraseña(contraseña))
            {
                ViewBag.Error = "Mail o contraseña incorrectos";
                return View();
            }

            // Crear claims personalizados
            var claims = new List<Claim>
    {
        new Claim("UsuarioId", usuario.Id.ToString())
    };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            // Iniciar sesión con cookie
            await HttpContext.SignInAsync("Cookies", principal);

            // Redirigir según tipo de usuario
            return usuario switch
            {
                Administrador => RedirectToAction("Dashboard", "Administrador"),
                Cliente => RedirectToAction("Dashboard", "Cliente"),
                Peluquero => RedirectToAction("Agenda", "Peluquero"),
                _ => RedirectToAction("Login")
            };
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string nombre, string apellido, string mail, string dni, DateTime fechaDeNacimiento, string contraseña)
        {
            if (_contexto.Personas.Any(u => u.Mail == mail))
            {
                ViewBag.Error = "El mail ya está registrado";
                return View();
            }

            var nuevoUsuario = UsuarioFactory.CrearUsuario<Cliente>(
                nombre, apellido, mail, dni, fechaDeNacimiento, contraseña);

            // BUSCAR GRUPO "Cliente"
            var grupoCliente = _contexto.Grupos.FirstOrDefault(g => g.Nombre == "Cliente");

            if (grupoCliente == null)
            {
                ViewBag.Error = "No existe el grupo 'Cliente'. Debes crearlo antes.";
                return View();
            }

            // ASIGNAR GRUPO AL NUEVO USUARIO
            nuevoUsuario.Grupos.Add(grupoCliente);

            // GUARDAR USUARIO
            _contexto.Personas.Add(nuevoUsuario);
            _contexto.SaveChanges();

            return RedirectToAction("Login");
        }



        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }
    }
}
