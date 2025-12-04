using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeAdministrador;

namespace PeluqueriaCanina.Controllers
{
    public class ServicioController : Controller
    {
        private readonly ContextoAcqua _contexto;
        public ServicioController(ContextoAcqua contexto)
        {
            _contexto = contexto;
        }
        [PermisoRequerido("VerServicios")]
        public IActionResult Index()
        {
            var servicios = _contexto.Servicios.ToList();
            return View(servicios);
        }
        [HttpGet]
        [PermisoRequerido("RegistrarServicio")]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [PermisoRequerido("RegistrarServicio")]
        public IActionResult Crear(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _contexto.Servicios.Add(servicio);
                _contexto.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(servicio);
        }

        [HttpGet]
        [PermisoRequerido("ModificarServicio")]
        public IActionResult Editar(int id)
        {
            var servicio = _contexto.Servicios.Find(id);
            if (servicio == null) return NotFound();
            return View(servicio);
        }
        [HttpPost]
        [PermisoRequerido("ModificarServicio")]
        public IActionResult Editar(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _contexto.Servicios.Update(servicio);
                _contexto.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(servicio);
        }

        [HttpGet]
        [PermisoRequerido("EliminarServicio")]
        public IActionResult Eliminar(int id)
        {
            var servicio = _contexto.Servicios.Find(id);
            if (servicio == null) return NotFound();
            return View(servicio);
        }
        [HttpPost, ActionName("Eliminar")]
        [PermisoRequerido("EliminarServicio")]
        public IActionResult EliminarConfirmado(int id)
        {
            var servicio = _contexto.Servicios.Find(id);
            if (servicio == null) return NotFound();

            _contexto.Servicios.Remove(servicio);
            _contexto.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
