using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Data;

namespace PeluqueriaCanina.Controllers
{
    public class ClienteController:Controller
    {
        private readonly ContextoAcqua _context;
        public ClienteController(ContextoAcqua contexto)
        {
            _context = contexto;
        }
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("Rol")!="Cliente") return RedirectToAction("Login", "Auth");

            var nombre = HttpContext.Session.GetString("Nombre");
            ViewBag.Nombre = string.IsNullOrEmpty(nombre) ? "Usuario" : nombre;

            return View();
        }
    }
}
