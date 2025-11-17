namespace PeluqueriaCanina.Models.REPORTES
{
    // Mapea la vista vw_ReporteDetallePeluquero (NombreServicio, NombrePeluquero, Realizados, Cancelados, Recaudado)
    public class ReporteDetallePeluqueroView
    {
        public string NombreServicio { get; set; } = string.Empty;
        public string NombrePeluquero { get; set; } = string.Empty;
        public int Realizados { get; set; }
        public int Cancelados { get; set; }
        public decimal Recaudado { get; set; }
    }
}
