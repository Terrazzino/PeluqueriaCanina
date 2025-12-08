using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.Users;
using PeluqueriaCanina.Services;
using System.Security.Claims;

namespace PeluqueriaCanina.Controllers
{
    public class AuthController : Controller
    {
        private readonly ContextoAcqua _contexto;
        private readonly AdminEntryStrategy _adminStrategy;
        private readonly ClienteEntryStrategy _clienteStrategy;
        private readonly PeluqueroEntryStrategy _peluqueroStrategy;
        private readonly AuditoriaService _auditoriaService;


        public AuthController(ContextoAcqua contexto,
        AdminEntryStrategy adminStrategy,
        ClienteEntryStrategy clienteStrategy,
        PeluqueroEntryStrategy peluqueroStrategy,
        AuditoriaService auditoriaService)
        {
            _contexto = contexto;
            _adminStrategy = adminStrategy;
            _clienteStrategy = clienteStrategy;
            _peluqueroStrategy = peluqueroStrategy;
            _auditoriaService = auditoriaService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string mail, string contraseña)
        {
            var usuario = _contexto.Personas
                .Include(u => u.Grupos)
                    .ThenInclude(g => g.Permisos)
                .FirstOrDefault(u => u.Mail == mail);

            if (usuario == null || !usuario.VerificarContraseña(contraseña))
            {
                ViewBag.Error = "Mail o contraseña incorrectos";
                return View();
            }

            // Guardar UsuarioId en Claims (esto reemplaza Session correctamente)
            var claims = new List<Claim>
            {
                new Claim("UsuarioId", usuario.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("Cookies", principal);


            // 🔥 Si tiene más de 1 grupo → debe elegir su rol
            if (usuario.Grupos.Count > 1)
            {
                // Info que necesita la vista para mostrar los roles disponibles
                ViewBag.UsuarioId = usuario.Id;
                ViewBag.Grupos = usuario.Grupos.Select(g => g.Nombre).ToList();

                // Mostrar la vista SeleccionarPerfil (tu versión del “SeleccionarRol”)
                return View("SeleccionarPerfil");
            }


            // 🔥 Si solo tiene un rol → lo enviamos directo por Strategy
            var grupoUnico = usuario.Grupos.FirstOrDefault()?.Nombre;

            _auditoriaService.Registrar(
                accion: "Login",
                usuarioId: usuario.Id,
                detalles: $"El usuario inició sesión."
            );



            return grupoUnico switch
            {
                "Administrador" => _adminStrategy.Execute(this, usuario),
                "Cliente" => _clienteStrategy.Execute(this, usuario),
                "Peluquero" => _peluqueroStrategy.Execute(this, usuario),
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

            _auditoriaService.Registrar(
                accion: "Registro de Usuario",
                usuarioId: nuevoUsuario.Id,
                detalles: $"Usuario creado como Rol {nuevoUsuario.Rol}"
            );  


            return RedirectToAction("Login");
        }

        // ---- NUEVA ACCIÓN ----
        [HttpPost]
        public IActionResult EntrarComo(int usuarioId, string perfil)
        {
            var usuario = _contexto.Personas
                .Include(u => u.Grupos)
                .FirstOrDefault(u => u.Id == usuarioId);

            if (usuario == null) return RedirectToAction("Login");

            return perfil switch
            {
                "Administrador" => _adminStrategy.Execute(this, usuario),
                "Cliente" => _clienteStrategy.Execute(this, usuario),
                "Peluquero" => _peluqueroStrategy.Execute(this, usuario),
                _ => RedirectToAction("Login")
            };
        }

        public async Task<IActionResult> Logout()
        {
            // 🔥 Recuperar el UsuarioId desde las claims
            var usuarioIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UsuarioId");
            int usuarioId = usuarioIdClaim != null ? int.Parse(usuarioIdClaim.Value) : 0;

            // Registrar auditoría solo si había un usuario logueado
            if (usuarioId > 0)
            {
                _auditoriaService.Registrar(
                    accion: "Logout",
                    usuarioId: usuarioId,
                    detalles: "El usuario cerró sesión."
                );
            }

            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }


    }
}
