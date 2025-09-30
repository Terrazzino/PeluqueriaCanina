namespace PeluqueriaCanina.Models.Permisos
{
    public class PermisoSimple:Permiso
    {
        public override bool TienePermiso(string accion) => accion == Nombre;
    }
}
