using System.ComponentModel.DataAnnotations;

namespace PeluqueriaCanina.Models.ViewModels
{
    public class EditarPersonaViewModel
    {
        public int Id { get; set; }

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
        [DataType(DataType.Date)]
        public DateTime FechaDeNacimiento { get; set; }
    }
}
