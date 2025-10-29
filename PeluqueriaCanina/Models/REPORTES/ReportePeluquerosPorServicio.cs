namespace PeluqueriaCanina.Models.REPORTES
{
    public class ReportePeluquerosPorServicio
    {
        public int Id { get; set; }
        public string NombreServicio { get; set; } = "";
        public string NombrePeluquero { get; set; } = "";
        public int Cantidad { get; set; }
    }
}
