using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.Users;
using PeluqueriaCanina.Models.Factories;

namespace PeluqueriaCanina.Controllers
{
    public class AuthController:Controller
    {
        private readonly ContextoAcqua _contexto;

        public AuthController(ContextoAcqua context)
        {
            _contexto = context;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //POST: /Auth/Login
        [HttpPost]
        public IActionResult Login(string mail, string contraseña)
        {
            var usuario = _contexto.Personas.FirstOrDefault(u => u.Mail == mail);
            if (usuario == null || !usuario.VerificarContraseña(contraseña))
            {
                ViewBag.Error = "Mail o contraseña incorrectos";
                return View();
            }
            HttpContext.Session.SetString("UsuarioId",usuario.Id.ToString());
            usuario.Permisos = PermisoFactory.CrearPermiso(usuario.Rol);

            switch (usuario.Rol)
            {
                case "Administrador":
                    return RedirectToAction("Dashboard","Administrador");
                case "Cliente":
                    return RedirectToAction("Dashboard", "Cliente");
                case "Peluquero":
                    return RedirectToAction("Agenda","Peluquero");
                default:
                    throw new ArgumentException("No se identifico ningun tipo de usuario");
            }
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        public IActionResult Register(string nombre, string apellido, string mail, string dni, DateTime fechaDeNacimiento, string contraseña)
        {
            if (_contexto.Personas.Any(u=>u.Mail==mail))
            {
                ViewBag.Error = "El mail ya esta registrado";
                return View();
            }

            var nuevoUsuario = UsuarioFactory.CrearUsuario<Cliente>(nombre, apellido, mail, dni, fechaDeNacimiento, contraseña, "Cliente");

            _contexto.Personas.Add(nuevoUsuario);
            _contexto.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        
    }
}
