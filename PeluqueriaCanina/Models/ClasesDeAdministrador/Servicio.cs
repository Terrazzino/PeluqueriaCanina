namespace PeluqueriaCanina.Models.ClasesDeAdministrador
{
    public class Servicio
    {
        public int Id { get; set; }
        public string NombreDelServicio { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public TimeSpan Duracion { get; set; }
    }
}