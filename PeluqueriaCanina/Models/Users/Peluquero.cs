using PeluqueriaCanina.Models.ClasesDeCliente;
using PeluqueriaCanina.Models.ClasesDePeluquero;
using PeluqueriaCanina.Models.ClasesDeTurno;
using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models.Users
{
    public class Peluquero : Persona
    {
        public Peluquero()
        {
            Rol = "Peluquero";
        }

    }
}
