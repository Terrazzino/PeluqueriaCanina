using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models.Users
{
    public class Administrador : Persona
    {
        public Administrador()
        {
            Rol = "Administrador";
        }
    }
}
