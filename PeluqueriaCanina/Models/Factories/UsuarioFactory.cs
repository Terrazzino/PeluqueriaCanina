using PeluqueriaCanina.Models.Users;

namespace PeluqueriaCanina.Models.Factories
{
    public class UsuarioFactory
    {
        public static T CrearUsuario<T>(
            string nombre, 
            string apellido, 
            string mail, 
            string dni, 
            DateTime fechaDeNacimiento, 
            string contraseña, 
            string rol) where T : Persona, new()
        {
            var usuario = new T
            {
                Nombre = nombre,
                Apellido = apellido,
                Mail = mail,
                Dni = dni,
                FechaDeNacimiento = fechaDeNacimiento
            };

            usuario.RegistrarContraseña(contraseña);

            usuario.Rol = typeof(T).Name;
            usuario.Permisos = PermisoFactory.CrearPermiso(rol);

            return usuario;
        }
    }
}
