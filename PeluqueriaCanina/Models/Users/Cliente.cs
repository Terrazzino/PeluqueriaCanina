using PeluqueriaCanina.Models.ClasesDeCliente;

namespace PeluqueriaCanina.Models.Users
{
    public class Cliente : Persona
    {
        public List<Mascota> Mascotas { get; set; } = new List<Mascota>();
    }
}
