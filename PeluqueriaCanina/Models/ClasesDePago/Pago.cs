using PeluqueriaCanina.Models.ClasesDeTurno;
using System;
using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models.ClasesDePago
{
    public class Pago
    {
        public int Id { get; set; }

        [Required]
        public int TurnoId { get; set; }
        public Turno Turno { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
        public decimal Monto { get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; }

        [Required]
        public EstadoPago Estado { get; set; } = EstadoPago.Pendiente;

        [Required]
        public DateTime FechaPago { get; set; } = DateTime.Now;

        // Campos opcionales para información adicional
        [StringLength(100)]
        public string? NumeroTransaccion { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        // Para pagos con tarjeta (últimos 4 dígitos)
        [StringLength(4)]
        public string? Ultimos4Digitos { get; set; }

        // Para identificar quien procesó el pago (puede ser el cliente o un admin)
        public int? ProcesadoPorUsuarioId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }
    }
}