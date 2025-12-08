using PeluqueriaCanina.Models;
using PeluqueriaCanina.Data;
using Microsoft.EntityFrameworkCore;

namespace PeluqueriaCanina.Services
{
    public interface IAuditoriaService
    {
        void Registrar(string accion, int usuarioId, string? detalles = null);
    }

    public class AuditoriaService : IAuditoriaService
    {
        private readonly ContextoAcqua _context;

        public AuditoriaService(ContextoAcqua context)
        {
            _context = context;
        }

        public void Registrar(string accion, int usuarioId, string? detalles = null)
        {
            var usuario = _context.Personas.FirstOrDefault(x => x.Id == usuarioId);
            if (usuario == null) return;

            var audit = new Auditoria
            {
                Accion = accion,
                UsuarioId = usuarioId,
                NombreUsuario = $"{usuario.Nombre} {usuario.Apellido}",
                RolUsuario = usuario.Rol,
                Detalles = detalles ?? ""
            };

            _context.Auditorias.Add(audit);
            _context.SaveChanges();
        }
    }
}
