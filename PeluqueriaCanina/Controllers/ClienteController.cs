using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Services;

namespace PeluqueriaCanina.Controllers
{
    public class ClienteController:Controller
    {
        private readonly ContextoAcqua _context;
        private readonly IUsuarioActualService _usuarioActual;
        public ClienteController(ContextoAcqua contexto, IUsuarioActualService usuarioActual)
        {
            _context = contexto;
            _usuarioActual = usuarioActual;
        }

        [PermisoRequerido("AccederDashboardCliente")]
        public IActionResult Dashboard()
        {
            ViewBag.Nombre = _usuarioActual.Obtener().Nombre;

            return View();
        }
    }
}
