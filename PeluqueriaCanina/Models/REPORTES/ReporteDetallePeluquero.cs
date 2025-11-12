using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models.REPORTES
{
    public class ReporteDetallePeluquero
    {
        [Key]
        public int Id { get; set; }

        public int ReportePeluqueroPorServicioId { get; set; } // FK -> ReportePeluqueroPorServicio.Id
        public int Realizados { get; set; }
        public int Cancelados { get; set; }
        public decimal Recaudado { get; set; }

        // navegación opcional
        public ReportePeluquerosPorServicio? ReportePeluqueroPorServicio { get; set; }
    }
}
