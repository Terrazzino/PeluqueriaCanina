using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Services;

namespace PeluqueriaCanina.Controllers
{
    public class ValoracionesController : Controller
    {
        private readonly ContextoAcqua _contexto;
        private readonly IUsuarioActualService _usuarioActual;

        public ValoracionesController(ContextoAcqua contexto, IUsuarioActualService usuarioActual)
        {
            _contexto = contexto;
            _usuarioActual = usuarioActual;
        }

        [HttpGet]
        [PermisoRequerido("RegistrarPuntuacionPeluquero")]
        public IActionResult Crear(int turnoId)
        {
            var turno = _contexto.Turnos
                .Include(t => t.Peluquero)
                .Include(t => t.Mascota)
                    .ThenInclude(m => m.Cliente)
                .FirstOrDefault(t => t.Id == turnoId);

            if (turno == null) return NotFound();
            if (turno.Estado != EstadoTurno.Completado) return BadRequest("El turno no está completado.");
            if (turno.Valoracion != null) return BadRequest("Este turno ya fue valorado.");

            var model = new Valoracion
            {
                TurnoId = turno.Id,
                PeluqueroId = turno.PeluqueroId,
                ClienteId = turno.Mascota.ClienteId
            };

            return View(model);
        }



        [HttpPost]
        [PermisoRequerido("RegistrarPuntuacionPeluquero")]
        public IActionResult Crear(Valoracion model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.FechaValoracion = DateTime.Now;

            _contexto.Valoraciones.Add(model);
            _contexto.SaveChanges();

            return RedirectToAction("Index", "Turno");
        }


        // ADMINISTRADOR → Lista TODAS las valoraciones
        [PermisoRequerido("ListarPuntuacionPeluquero")]
        public IActionResult ListadoGeneral()
        {
            var valoraciones = _contexto.Valoraciones
                .Include(v => v.Peluquero)
                .Include(v => v.Cliente)
                .ToList();

            return View(valoraciones);
        }
    }
}
