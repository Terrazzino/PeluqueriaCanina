namespace PeluqueriaCanina.Models.ClasesDeCliente;

using PeluqueriaCanina.Models.ClasesDeTurno;
using PeluqueriaCanina.Models.Users;

public class Valoracion
{
    public int Id { get; set; }

    public int PeluqueroId { get; set; }
    public Peluquero Peluquero { get; set; }

    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; }

    public int TurnoId { get; set; }
    public Turno Turno { get; set; }

    public int Puntuacion { get; set; } // 1 a 5
    public string? Comentario { get; set; }
    public DateTime FechaValoracion { get; set; } = DateTime.Now;
}
