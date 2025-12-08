public abstract class Permiso
{
    public int Id { get; set; }
    public string Nombre { get; set; }

    public abstract bool TienePermiso(string nombre);
    public abstract List<string> ListarPermisos();
}
