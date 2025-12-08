using PeluqueriaCanina.Models.ClasesDeCliente;
using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models.Users
{
    public class Cliente : Persona
    {
        public Cliente()
        {
            Rol = "Cliente";
        }
        public List<Mascota> Mascotas { get; set; } = new List<Mascota>();
        public List<Valoracion> Valoraciones { get; set; } = new();

    }
}
