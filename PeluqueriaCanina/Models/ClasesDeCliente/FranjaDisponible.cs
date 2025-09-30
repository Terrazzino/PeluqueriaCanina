using PeluqueriaCanina.Models.ClasesDePeluquero;

namespace PeluqueriaCanina.Models.ClasesDeCliente
{
    public class FranjaDisponible
    {
        public int PeluqueroId { get; set; }
        public string PeluqueroNombre { get; set; }
        public DiasLaborales Dia {  get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin {  get; set; }
        public bool Libre {  get; set; }
    }
}
