using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models.Factories;
using PeluqueriaCanina.Models.Users;

namespace PeluqueriaCanina.Data
{
    public class DbInitializer
    {
        public static void Initialize(ContextoAcqua context)
        {
            context.Database.Migrate();
            if (context.Administradores.Any())
                return;
            var admin = UsuarioFactory.CrearUsuario<Administrador>(
                nombre: "Leo",
                apellido: "Fender",
                dni: "20564788",
                mail: "gibsonselacome@gmail.com",
                fechaDeNacimiento: new DateTime(1909, 8, 10),
                contraseña: "Fender90",
                rol: "Administrador"
                );
            context.Administradores.Add( admin );
            context.SaveChanges();
        }
    }
}
