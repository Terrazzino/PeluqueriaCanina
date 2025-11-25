namespace PeluqueriaCanina.Models.VMs
{
    public class PeluqueroRankingVM
    {
        public int PeluqueroId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public double Promedio { get; set; }
        public int Cantidad { get; set; }
    }
}
