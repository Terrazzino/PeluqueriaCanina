namespace PeluqueriaCanina.Models.REPORTES
{
    // Mapea la vista vw_ReportePeluquerosPorServicio (NombreServicio, NombrePeluquero, Cantidad)
    public class ReportePeluquerosPorServicio
    {
        public string NombreServicio { get; set; } = string.Empty;
        public string NombrePeluquero { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}
