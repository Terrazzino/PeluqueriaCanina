namespace PeluqueriaCanina.Models.ClasesDeCliente;

using System.ComponentModel.DataAnnotations;
using PeluqueriaCanina.Models.ClasesDeTurno;
using PeluqueriaCanina.Models.Users;

public class Valoracion
{
    public int Id { get; set; }

    public int? PeluqueroId { get; set; }
    public Persona? Peluquero { get; set; }      // <- nullable

    public int ClienteId { get; set; }
    public Persona? Cliente { get; set; }         // <- nullable

    public int TurnoId { get; set; }
    public Turno? Turno { get; set; }             // <- nullable

    [Required]
    [Range(1, 5)]
    public int Puntuacion { get; set; } // 1 a 5

    public string? Comentario { get; set; }

    public DateTime FechaValoracion { get; set; } = DateTime.Now;
}
