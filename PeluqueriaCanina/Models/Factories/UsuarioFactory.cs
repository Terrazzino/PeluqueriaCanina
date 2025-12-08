using PeluqueriaCanina.Models.Users;

public class UsuarioFactory
{
    public static T CrearUsuario<T>(
        string nombre,
        string apellido,
        string mail,
        string dni,
        DateTime fechaDeNacimiento,
        string contraseña
    ) where T : Persona, new()
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

        return usuario;
    }
}
