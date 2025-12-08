using PeluqueriaCanina.Models.Users;

namespace PeluqueriaCanina.Models
{
    public class Auditoria
    {
        public int Id { get; set; }

        public DateTime FechaHora { get; set; } = DateTime.Now;

        // Usuario que realizó la acción
        public int UsuarioId { get; set; }
        public Persona Usuario { get; set; }

        public string NombreUsuario { get; set; } = string.Empty;
        public string RolUsuario { get; set; } = string.Empty;

        // Acción registrada
        public string Accion { get; set; } = string.Empty;

        // Información adicional (opcional)
        public string Detalles { get; set; } = string.Empty;
    }
}
