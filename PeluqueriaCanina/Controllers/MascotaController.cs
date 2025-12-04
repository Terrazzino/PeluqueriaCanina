using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Services;

namespace PeluqueriaCanina.Controllers
{
    public class MascotaController:Controller
    {
        private readonly ContextoAcqua _contexto;
        private readonly IUsuarioActualService _usuarioActual;
        public MascotaController(ContextoAcqua contexto, IUsuarioActualService usuarioActual)
        {
            _contexto = contexto;
            _usuarioActual = usuarioActual;
        }

        [PermisoRequerido("AccederDashboardCliente")]
        public IActionResult Index()
        {
            var clienteId = _usuarioActual.Obtener().Id;
            var mascotas = _contexto.Mascotas
                .Where(m => m.ClienteId == clienteId)
                .ToList();
            return View(mascotas);
        }

        [HttpGet]
        [PermisoRequerido("RegistrarMascota")]
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        [PermisoRequerido("RegistrarMascota")]
        public IActionResult Crear(Mascota mascota)
        {
            mascota.ClienteId = int.Parse(HttpContext.Session.GetString("UsuarioId"));
            _contexto.Mascotas.Add(mascota);
            _contexto.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [PermisoRequerido("ModificarMascota")]
        public IActionResult Editar(int id)
        {
            var mascota = _contexto.Mascotas.Find(id);
            if (mascota == null) return NotFound();
            return View(mascota);
        }

        [HttpPost]
        [PermisoRequerido("ModificarMascota")]
        public IActionResult Editar(Mascota nuevaMascota)
        {
            var mascota = _contexto.Mascotas.Find(nuevaMascota.Id);
            mascota.Nombre = nuevaMascota.Nombre;
            mascota.FechaDeNacimiento = nuevaMascota.FechaDeNacimiento;
            mascota.Raza = nuevaMascota.Raza;
            mascota.Peso = nuevaMascota.Peso;

            _contexto.Mascotas.Update(mascota);
            _contexto.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [PermisoRequerido("EliminarMascota")]
        public IActionResult Eliminar(int id)
        {
            var mascota = _contexto.Mascotas.Find(id);
            if (mascota == null) return NotFound();
            return View(mascota);
        }

        [HttpPost, ActionName("Eliminar")]
        [PermisoRequerido("EliminarMascota")]
        public IActionResult EliminarConfirmado(int id)
        {
            var mascota = _contexto.Mascotas.Find(id);
            if (mascota == null) return NotFound();
            _contexto.Mascotas.Remove(mascota);
            _contexto.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
