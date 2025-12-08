using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.Factories;
using PeluqueriaCanina.Models.Users;

public class DbInitializer
{
    public static void Initialize(ContextoAcqua context)
    {
        context.Database.Migrate();

        // 1) Crear grupos si no existen
        Grupo adminGroup = null;
        Grupo peluGroup = null;
        Grupo clienteGroup = null;

        if (!context.Grupos.Any())
        {
            adminGroup = new Grupo { Nombre = "Administrador" };
            peluGroup = new Grupo { Nombre = "Peluquero" };
            clienteGroup = new Grupo { Nombre = "Cliente" };

            adminGroup.Permisos = (PermisoCompuesto)PermisoFactory.CrearPermiso("Administrador");
            peluGroup.Permisos = (PermisoCompuesto)PermisoFactory.CrearPermiso("Peluquero");
            clienteGroup.Permisos = (PermisoCompuesto)PermisoFactory.CrearPermiso("Cliente");


            context.Grupos.AddRange(adminGroup, peluGroup, clienteGroup);
            context.SaveChanges();
        }
        else
        {
            adminGroup = context.Grupos.First(g => g.Nombre == "Administrador");
            peluGroup = context.Grupos.First(g => g.Nombre == "Peluquero");
            clienteGroup = context.Grupos.First(g => g.Nombre == "Cliente");
        }

        // 2) Crear administrador por defecto si no existe
        if (!context.Administradores.Any())
        {
            var admin = UsuarioFactory.CrearUsuario<Administrador>(
                nombre: "Paola",
                apellido: "Comino",
                dni: "20564788",
                mail: "acquadicane@gmail.com",
                fechaDeNacimiento: new DateTime(1909, 8, 10),
                contraseña: "Acqua2025"
            );

            admin.Grupos.Add(adminGroup);

            context.Administradores.Add(admin);
            context.SaveChanges();
        }
    }
}
