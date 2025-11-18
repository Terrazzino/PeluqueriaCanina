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
                nombre: "Paola",
                apellido: "Comino",
                dni: "20564788",
                mail: "acquadicane@gmail.com",
                fechaDeNacimiento: new DateTime(1909, 8, 10),
                contraseña: "Acqua2025",
                rol: "Administrador"
                );
            context.Administradores.Add( admin );
            context.SaveChanges();
        }
    }
}
