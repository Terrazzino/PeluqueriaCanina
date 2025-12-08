namespace PeluqueriaCanina.Models.ClasesDeTurno
{
    public interface IEstadoTurno
    {
        string Nombre { get; }

        bool PuedeModificar { get; }
        bool PuedeCancelar { get; }
        bool PuedeValorar { get; }
    }
}
