namespace PeluqueriaCanina.Models.REPORTES
{
    public class ReportePeluquerosPorServicio
    {
        public int Id { get; set; }
        public int ReporteServicioId { get; set; }      // FK -> ReporteServicios.Id
        public string NombrePeluquero { get; set; } = string.Empty;
        public int Cantidad { get; set; }

        // navegación opcional
        public ReporteServicios? ReporteServicio { get; set; }
        public ICollection<ReporteDetallePeluquero>? Detalles { get; set; }
    }
}
