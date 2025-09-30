namespace PeluqueriaCanina.Models.Permisos
{
    public class PermisoCompuesto:Permiso
    {
        private List<Permiso> _permisos = new List<Permiso>();
        public void AgregarPermiso(Permiso permiso)=>_permisos.Add(permiso);
        public override bool TienePermiso(string accion) => _permisos.Any(p=>p.TienePermiso(accion));
    }
}
