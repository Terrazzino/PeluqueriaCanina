using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeCliente;

namespace PeluqueriaCanina.Controllers
{
    public class MascotaController:Controller
    {
        private readonly ContextoAcqua _contexto;
        public MascotaController(ContextoAcqua contexto)
        {
            _contexto = contexto;
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Rol") != "Cliente") return RedirectToAction("Login", "Auth");

            var clienteId = int.Parse(HttpContext.Session.GetString("UsuarioId"));
            var mascotas = _contexto.Mascotas
                .Where(m => m.ClienteId == clienteId)
                .ToList();
            return View(mascotas);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Crear(Mascota mascota)
        {
            mascota.ClienteId = int.Parse(HttpContext.Session.GetString("UsuarioId"));
            _contexto.Mascotas.Add(mascota);
            _contexto.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var mascota = _contexto.Mascotas.Find(id);
            if (mascota == null) return NotFound();
            return View(mascota);
        }

        [HttpPost]
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
        public IActionResult Eliminar(int id)
        {
            var mascota = _contexto.Mascotas.Find(id);
            if (mascota == null) return NotFound();
            return View(mascota);
        }

        [HttpPost, ActionName("Eliminar")]
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
