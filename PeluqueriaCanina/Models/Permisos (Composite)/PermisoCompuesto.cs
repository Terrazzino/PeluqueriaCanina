public class PermisoCompuesto : Permiso
{
    public List<Permiso> Permisos { get; set; } = new();

    public void AgregarPermiso(Permiso permiso)
    {
        Permisos.Add(permiso);
    }

    public override bool TienePermiso(string nombre)
    {
        return Permisos.Any(p => p.Nombre == nombre || (p is PermisoCompuesto pc && pc.TienePermiso(nombre)));
    }

    public override List<string> ListarPermisos()
    {
        var lista = new List<string> { Nombre };

        foreach (var permiso in Permisos)
            lista.AddRange(permiso.ListarPermisos());

        return lista;
    }
}
