using PeluqueriaCanina.Models.Users;

namespace PeluqueriaCanina.Models.ClasesDePeluquero
{
    public class Jornada
    {
        public int Id { get; set; }
        public DiasLaborales Dia { get; set; }
        public TimeSpan HoraDeInicio { get; set; }
        public TimeSpan HoraDeFin {  get; set; }
        public bool Activo {  get; set; }
        public int PeluqueroId { get; set; }
        public Peluquero Peluquero { get; set; }
    }
}
