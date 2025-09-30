using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models.Users
{
    public abstract class Persona : Usuario
    {
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Apellido { get; set; } = string.Empty;
        [Required]
        [StringLength(8)]
        public string Dni { get; set; } = string.Empty;
        [Required]
        public DateTime FechaDeNacimiento { get; set; }
    }
}
