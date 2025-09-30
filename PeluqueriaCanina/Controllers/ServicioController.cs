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
        public IActionResult Index()
        {
            var servicios = _contexto.Servicios.ToList();
            return View(servicios);
        }
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
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
        public IActionResult Editar(int id)
        {
            var servicio = _contexto.Servicios.Find(id);
            if (servicio == null) return NotFound();
            return View(servicio);
        }
        [HttpPost]
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
        public IActionResult Eliminar(int id)
        {
            var servicio = _contexto.Servicios.Find(id);
            if (servicio == null) return NotFound();
            return View(servicio);
        }
        [HttpPost, ActionName("Eliminar")]
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
