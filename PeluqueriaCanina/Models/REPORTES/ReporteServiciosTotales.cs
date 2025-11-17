namespace PeluqueriaCanina.Models.REPORTES
{
    // Mapea la vista vw_ReporteServiciosTotales (Id, NombreServicio, Total)
    public class ReporteServiciosTotales
    {
        public int Id { get; set; }
        public string NombreServicio { get; set; } = string.Empty;
        public int Total { get; set; }
    }
}
