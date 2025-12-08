using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.Users;

namespace PeluqueriaCanina.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ContextoAcqua _context;

        public UsuarioController(ContextoAcqua context)
        {
            _context = context;
        }

        public IActionResult Index(string tipo = "Todos")
        {
            var usuarios = _context.Personas
                .Include(u => u.Grupos)
                .ToList();

            ViewBag.TipoSeleccionado = tipo;

            usuarios = tipo switch
            {
                "Clientes" => usuarios.Where(u => u is Cliente).ToList(),
                "Peluqueros" => usuarios.Where(u => u is Peluquero).ToList(),
                "Administradores" => usuarios.Where(u => u is Administrador).ToList(),
                _ => usuarios
            };

            return View(usuarios);
        }

        public IActionResult EditarRoles(int id)
        {
            var user = _context.Personas
                .Include(u => u.Grupos)
                .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            ViewBag.GruposDisponibles = _context.Grupos.ToList();

            return View(user);
        }

        [HttpPost]
        public IActionResult AgregarGrupo(int usuarioId, int grupoId)
        {
            var user = _context.Personas
                .Include(u => u.Grupos)
                .FirstOrDefault(u => u.Id == usuarioId);

            var grupo = _context.Grupos
                .FirstOrDefault(g => g.Id == grupoId);

            if (user == null || grupo == null) return NotFound();

            if (!user.Grupos.Any(g => g.Id == grupoId))
                user.Grupos.Add(grupo);

            _context.SaveChanges();

            return RedirectToAction("EditarRoles", new { id = usuarioId });
        }

        public IActionResult QuitarGrupo(int usuarioId, int grupoId)
        {
            var user = _context.Personas
                .Include(u => u.Grupos)
                .FirstOrDefault(u => u.Id == usuarioId);

            if (user == null) return NotFound();

            var grupo = user.Grupos.FirstOrDefault(g => g.Id == grupoId);

            if (grupo != null)
            {
                user.Grupos.Remove(grupo);
                _context.SaveChanges();
            }

            return RedirectToAction("EditarRoles", new { id = usuarioId });
        }
    }
}
