using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeCliente;

namespace PeluqueriaCanina.Controllers
{
    public class ValoracionesController : Controller
    {
        private readonly ContextoAcqua _contexto;

        public ValoracionesController(ContextoAcqua contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult Crear(int turnoId)
        {
            var turno = _contexto.Turnos
                .Include(t => t.Peluquero)
                .Include(t => t.Mascota.Cliente)
                .FirstOrDefault(t => t.Id == turnoId);

            if (turno == null) return NotFound();
            if (turno.Estado != EstadoTurno.Completado) return BadRequest("El turno no está completado.");
            if (turno.Valoracion != null) return BadRequest("Este turno ya fue valorado.");

            var model = new Valoracion
            {
                TurnoId = turnoId
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Crear(Valoracion model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.FechaValoracion = DateTime.Now;

            _contexto.Valoraciones.Add(model);
            _contexto.SaveChanges();

            return RedirectToAction("MisTurnos", "Cliente");
        }


        // ADMINISTRADOR → Lista TODAS las valoraciones
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
