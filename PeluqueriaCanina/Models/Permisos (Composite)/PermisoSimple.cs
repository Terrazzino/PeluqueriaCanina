public class PermisoSimple : Permiso
{
    public override bool TienePermiso(string nombre)
    {
        return Nombre == nombre;
    }

    public override List<string> ListarPermisos()
    {
        return new List<string> { Nombre };
    }
}
