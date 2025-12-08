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
    }
}
