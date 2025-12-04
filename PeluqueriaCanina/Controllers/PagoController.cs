using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDePago;
using PeluqueriaCanina.Services;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PeluqueriaCanina.Controllers
{
    public class PagoController : Controller
    {
        private readonly ContextoAcqua _contexto;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUsuarioActualService _usuarioActual;

        public PagoController(ContextoAcqua contexto, IConfiguration configuration, IHttpClientFactory httpClientFactory, IUsuarioActualService usuarioActual)
        {
            _contexto = contexto;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _usuarioActual = usuarioActual;
        }

        // GET: Pago/Procesar/5
        [HttpGet]
        public async Task<IActionResult> Procesar(int id)
        {
            var turno = await _contexto.Turnos
                .Include(t => t.Mascota)
                .Include(t => t.Servicio)
                .Include(t => t.Peluquero)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (turno == null)
            {
                return NotFound(new { ok = false, message = "El turno no existe." });
            }

            if (turno.Estado == EstadoTurno.Cancelado)
            {
                return BadRequest(new { ok = false, message = "No se puede pagar un turno cancelado." });
            }

            var pagoExistente = await _contexto.Pagos
                .FirstOrDefaultAsync(p => p.TurnoId == id && p.Estado == EstadoPago.Aprobado);
            turno.Estado = EstadoTurno.Confirmado;

            if (pagoExistente != null)
            {
                return BadRequest(new { ok = false, message = "Este turno ya ha sido pagado." });
            }

            ViewBag.Turno = turno;
            return View();
        }

        // POST: Pago/CrearPreferenciaMercadoPago
        [HttpPost]
        public async Task<IActionResult> CrearPreferenciaMercadoPago([FromBody] int turnoId)
        {
            try
            {
                var turno = await _contexto.Turnos
                    .Include(t => t.Mascota)
                        .ThenInclude(m => m.Cliente)
                    .Include(t => t.Servicio)
                    .Include(t => t.Peluquero)
                    .FirstOrDefaultAsync(t => t.Id == turnoId);

                if (turno == null)
                {
                    return Json(new { ok = false, message = "El turno no existe." });
                }

                // Crear el pago en estado Pendiente
                var pago = new Pago
                {
                    TurnoId = turnoId,
                    Monto = turno.Precio,
                    MetodoPago = MetodoPago.MercadoPago,
                    Estado = EstadoPago.Aprobado,
                    FechaPago = DateTime.Now,
                    FechaCreacion = DateTime.Now,
                    Observaciones = "Pago iniciado con Mercado Pago"
                };

                _contexto.Pagos.Add(pago);
                await _contexto.SaveChangesAsync();

                // Crear preferencia usando API REST de Mercado Pago
                var accessToken = _configuration["MercadoPago:AccessToken"];

                if (string.IsNullOrEmpty(accessToken))
                {
                    Console.WriteLine("Error: Access Token no configurado");
                    return Json(new { ok = false, message = "Error de configuración." });
                }

                var preferenceData = new
                {
                    items = new[]
                    {
                new
                {
                    title = $"{turno.Servicio.NombreDelServicio} - {turno.Mascota.Nombre}",
                    quantity = 1,
                    currency_id = "ARS",
                    unit_price = turno.Precio
                }
            },
                    back_urls = new
                    {
                        success = $"{Request.Scheme}://{Request.Host}/Pago/MercadoPagoSuccess?pagoId={pago.Id}",
                        failure = $"{Request.Scheme}://{Request.Host}/Pago/MercadoPagoFailure?pagoId={pago.Id}",
                        pending = $"{Request.Scheme}://{Request.Host}/Pago/MercadoPagoPending?pagoId={pago.Id}"
                    },
                    auto_return = "approved",
                    external_reference = pago.Id.ToString(),
                    statement_descriptor = "PELUQUERIA CANINA",
                    payer = new
                    {
                        name = turno.Mascota.Cliente?.Nombre ?? "Cliente",
                        email = turno.Mascota.Cliente?.Mail ?? "cliente@example.com"
                    }
                };

                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var jsonContent = JsonSerializer.Serialize(preferenceData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://api.mercadopago.com/checkout/preferences", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error de Mercado Pago: {responseContent}");
                    return Json(new { ok = false, message = "Error al crear preferencia en Mercado Pago." });
                }

                var preferenceResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                // ✅ CORRECCIÓN AQUÍ
                string? preferenceId = null;
                string? initPoint = null;

                if (preferenceResponse.TryGetProperty("id", out var idElement))
                {
                    preferenceId = idElement.GetString();
                }

                if (preferenceResponse.TryGetProperty("init_point", out var initElement))
                {
                    initPoint = initElement.GetString();
                }

                if (string.IsNullOrEmpty(preferenceId) || string.IsNullOrEmpty(initPoint))
                {
                    Console.WriteLine($"Respuesta de MercadoPago inválida: {responseContent}");
                    return Json(new { ok = false, message = "Error al procesar respuesta de Mercado Pago." });
                }

                // Guardar el ID de preferencia en el pago
                pago.NumeroTransaccion = preferenceId;
                await _contexto.SaveChangesAsync();

                return Json(new
                {
                    ok = true,
                    initPoint = initPoint,
                    pagoId = pago.Id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear preferencia de Mercado Pago: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return Json(new { ok = false, message = $"Error: {ex.Message}" });
            }
        }
        // GET: Pago/MercadoPagoSuccess
        public async Task<IActionResult> MercadoPagoSuccess(int pagoId, string payment_id, string status)
        {
            try
            {
                var pago = await _contexto.Pagos
                    .Include(p => p.Turno)
                    .FirstOrDefaultAsync(p => p.Id == pagoId);

                if (pago != null)
                {
                    pago.Estado = EstadoPago.Aprobado;
                    pago.FechaActualizacion = DateTime.Now;
                    pago.Observaciones += $" | Payment ID: {payment_id}";

                    pago.Turno.Estado = EstadoTurno.Confirmado;

                    await _contexto.SaveChangesAsync();
                }

                return RedirectToAction("Comprobante", new { id = pagoId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en MercadoPagoSuccess: {ex.Message}");
                return RedirectToAction("Index", "Turno");
            }
        }

        // GET: Pago/MercadoPagoFailure
        public async Task<IActionResult> MercadoPagoFailure(int pagoId)
        {
            try
            {
                var pago = await _contexto.Pagos.FindAsync(pagoId);

                if (pago != null)
                {
                    pago.Estado = EstadoPago.Aprobado;
                    pago.FechaActualizacion = DateTime.Now;
                  
                    pago.Observaciones += " | Pago rechazado en Mercado Pago";

                    await _contexto.SaveChangesAsync();
                }

                TempData["Error"] = "El pago fue rechazado. Por favor intenta nuevamente.";
                return RedirectToAction("Index", "Turno");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en MercadoPagoFailure: {ex.Message}");
                return RedirectToAction("Index", "Turno");
            }
        }

        // GET: Pago/MercadoPagoPending
        public async Task<IActionResult> MercadoPagoPending(int pagoId)
        {
            try
            {
                var pago = await _contexto.Pagos.FindAsync(pagoId);

                if (pago != null)
                {
                    pago.Estado = EstadoPago.Aprobado;
                    pago.FechaActualizacion = DateTime.Now;
                    pago.Observaciones += " | Pago pendiente en Mercado Pago";

                    await _contexto.SaveChangesAsync();
                }

                return RedirectToAction("Comprobante", new { id = pagoId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en MercadoPagoPending: {ex.Message}");
                return RedirectToAction("Index", "Turno");
            }
        }

        // POST: Pago/Procesar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Procesar(Pago pago)
        {
            try
            {
                var turno = await _contexto.Turnos
                    .Include(t => t.Servicio)
                    .FirstOrDefaultAsync(t => t.Id == pago.TurnoId);

                if (turno == null)
                {
                    return Json(new { ok = false, message = "El turno no existe." });
                }

                if (turno.Estado == EstadoTurno.Cancelado)
                {
                    return Json(new { ok = false, message = "No se puede pagar un turno cancelado." });
                }

                var pagoExistente = await _contexto.Pagos
                    .AnyAsync(p => p.TurnoId == pago.TurnoId && p.Estado == EstadoPago.Aprobado);

                if (pagoExistente)
                {
                    return Json(new { ok = false, message = "Este turno ya ha sido pagado." });
                }

                if (pago.Monto != turno.Precio)
                {
                    return Json(new { ok = false, message = $"El monto debe ser ${turno.Precio:N2}" });
                }

                pago.FechaPago = DateTime.Now;
                pago.FechaCreacion = DateTime.Now;

                if (pago.MetodoPago == MetodoPago.Efectivo || pago.MetodoPago == MetodoPago.Transferencia)
                {
                    pago.Estado = EstadoPago.Aprobado;
                    turno.Estado = EstadoTurno.Confirmado;
                }
                else
                {
                    pago.Estado = EstadoPago.Procesando;
                }

                _contexto.Pagos.Add(pago);
                await _contexto.SaveChangesAsync();

                return Json(new
                {
                    ok = true,
                    message = pago.Estado == EstadoPago.Aprobado
                        ? "Pago registrado exitosamente"
                        : "Pago en proceso de verificación",
                    pagoId = pago.Id
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar pago: {ex.Message}");
                return Json(new { ok = false, message = "Ocurrió un error al procesar el pago." });
            }
        }

        // GET: Pago/MisPagos
        [PermisoRequerido("MisPagos")]
        public async Task<IActionResult> MisPagos()
        {
            var clienteId = _usuarioActual.Obtener().Id;
            var pagos = await _contexto.Pagos
                .Include(p => p.Turno)
                    .ThenInclude(t => t.Mascota)
                .Include(p => p.Turno)
                    .ThenInclude(t => t.Servicio)
                .Where(p => p.Turno.Mascota.ClienteId == clienteId)
                .OrderByDescending(p => p.FechaPago)
                .ToListAsync();
            return View(pagos);
        }

        // GET: Pago/Comprobante/5
        public async Task<IActionResult> Comprobante(int id)
        {
            var pago = await _contexto.Pagos
                .Include(p => p.Turno)
                    .ThenInclude(t => t.Mascota)
                .Include(p => p.Turno)
                    .ThenInclude(t => t.Servicio)
                .Include(p => p.Turno)
                    .ThenInclude(t => t.Peluquero)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pago/Aprobar/5
        [HttpPost]
        public async Task<IActionResult> Aprobar(int id)
        {
            var pago = await _contexto.Pagos
                .Include(p => p.Turno)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null)
            {
                return Json(new { ok = false, message = "El pago no existe." });
            }

            if (pago.Estado == EstadoPago.Aprobado)
            {
                return Json(new { ok = false, message = "El pago ya está aprobado." });
            }

            pago.Estado = EstadoPago.Aprobado;
            pago.FechaActualizacion = DateTime.Now;
            pago.Turno.Estado = EstadoTurno.Confirmado;

            await _contexto.SaveChangesAsync();

            return Json(new { ok = true, message = "Pago aprobado exitosamente." });
        }

        // POST: Pago/Rechazar/5
        [HttpPost]
        public async Task<IActionResult> Rechazar(int id, string motivo)
        {
            var pago = await _contexto.Pagos.FindAsync(id);

            if (pago == null)
            {
                return Json(new { ok = false, message = "El pago no existe." });
            }

            if (pago.Estado == EstadoPago.Aprobado)
            {
                return Json(new { ok = false, message = "No se puede rechazar un pago ya aprobado." });
            }

            pago.Estado = EstadoPago.Rechazado;
            pago.Observaciones = motivo;
            pago.FechaActualizacion = DateTime.Now;

            await _contexto.SaveChangesAsync();

            return Json(new { ok = true, message = "Pago rechazado." });
        }
    }
}