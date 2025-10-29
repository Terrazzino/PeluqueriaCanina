namespace PeluqueriaCanina.Models.REPORTES
{
    public class ReporteDetallePeluquero
    {
        public int Id { get; set; }
        public string NombrePeluquero { get; set; } = "";
        public int Realizados { get; set; }
        public int Cancelados { get; set; }
        public decimal Recaudado { get; set; }
    }
}
