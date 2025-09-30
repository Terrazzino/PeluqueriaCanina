namespace PeluqueriaCanina.Models.Permisos
{
    public abstract class Permiso
    {
        public string Nombre { get; set; }
        public abstract bool TienePermiso(string accion);
    }
}
