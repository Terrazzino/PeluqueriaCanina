namespace PeluqueriaCanina.Models.Grupos;
using PeluqueriaCanina.Models.Users;

public class Grupo
{
    public int Id { get; set; }
    public string Nombre { get; set; }

    // Un grupo es un PermisoCompuesto
    public PermisoCompuesto Permisos { get; set; } = new PermisoCompuesto();

    // Relación muchos a muchos con Usuario
    public List<Usuario> Usuarios { get; set; } = new();
}
