using AcquaDiCane.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Data; // tu contexto de peluqueria
using PeluqueriaCanina.Models.ClasesDeCliente; // Mascota, Cliente
using PeluqueriaCanina.Services;
using System.Globalization;

public class VeterinariaController : Controller
{
    private readonly VeterinariaApiClient _api;
    private readonly ContextoAcqua _contexto;
    private readonly IUsuarioActualService _usuarioActual;

    public VeterinariaController(ContextoAcqua contexto, VeterinariaApiClient api, IUsuarioActualService usuarioActual)
    {
        _contexto = contexto;
        _api = api;
        _usuarioActual = usuarioActual;
    }

    // GET: /Veterinaria/Reservar  -> muestra selector mascota + fecha
    [PermisoRequerido("DashboardVeterinaria")]
    public IActionResult Reservar()
    {
        int clienteId = _usuarioActual.Obtener().Id;
        var mascotas = _contexto.Mascotas.Where(m => m.ClienteId == clienteId).ToList();
        return View(mascotas); // View expects List<Mascota>
    }

    // AJAX: Obtener disponibilidad (desde la API)
    [HttpGet]
    [PermisoRequerido("ReservarTurnoVeterinaria")]
    public async Task<IActionResult> Disponibilidad(DateTime fecha)
    {
        var horarios = await _api.ObtenerDisponibilidad(fecha);
        if (horarios == null) return StatusCode(500, "La API no respondió");
        return Json(horarios);
    }

    // POST: ReservarTurno -> la peluquería envía la ficha completa (cliente+mascota+fecha) a la API
    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermisoRequerido("ReservarTurnoVeterinaria")]
    public async Task<IActionResult> ReservarTurno(int mascotaId, DateTime fechaHora)
    {
        int clienteId = _usuarioActual.Obtener().Id;

        var mascota = await _contexto.Mascotas.FindAsync(mascotaId);
        var cliente = await _contexto.Clientes.FindAsync(clienteId);

        if (mascota == null || cliente == null) return BadRequest("Datos inválidos");

        var dto = new AcquaDiCane.Models.DTOs.TurnoCreateDto
        {
            Cliente = new ClienteVetDTO { Nombre = cliente.Nombre, Apellido = cliente.Apellido, Dni = cliente.Dni },
            Mascota = new MascotaVetDTO { Nombre = mascota.Nombre, Raza = mascota.Raza, Peso = double.Parse(mascota.Peso.ToString()) },
            FechaHora = fechaHora
        };

        var ok = await _api.CrearTurno(dto);
        if (!ok) return StatusCode(500, "No se pudo reservar el turno en la veterinaria");
        return Ok();
    }

    // GET: MisTurnos -> listados desde la API por DNI del cliente
    [PermisoRequerido("MisTurnosVeterinaria")]
    public async Task<IActionResult> MisTurnos()
    {
        int clienteId = _usuarioActual.Obtener().Id;

        var cliente = await _contexto.Clientes.FindAsync(clienteId);
        if (cliente == null) return RedirectToAction("Index", "Home");

        var turnos = await _api.ObtenerTurnosPorDni(cliente.Dni);
        return View(turnos);
    }

    // POST Cancelar (desde MisTurnos)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermisoRequerido("CancelarTurnoVeterinaria")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var ok = await _api.CancelarTurno(id);
        if (!ok) return StatusCode(500, "No se pudo cancelar");
        return Ok();
    }
}
