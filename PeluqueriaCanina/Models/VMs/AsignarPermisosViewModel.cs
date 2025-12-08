public class AsignarPermisosViewModel
{
    public int GrupoId { get; set; }

    public string NombreGrupo { get; set; }

    public List<string> PermisosDisponibles { get; set; } = new List<string>();

    public List<string> PermisosAsignados { get; set; } = new List<string>();
}
