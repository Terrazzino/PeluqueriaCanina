using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.Users;

namespace PeluqueriaCanina.Services
{
    public class UsuarioActualService : IUsuarioActualService
    {
        private readonly IHttpContextAccessor _http;
        private readonly ContextoAcqua _contexto;

        public UsuarioActualService(IHttpContextAccessor http, ContextoAcqua contexto)
        {
            _http = http;
            _contexto = contexto;
        }

        public Persona Obtener()
        {
            var claimValue = _http.HttpContext.User.FindFirst("UsuarioId")?.Value;

            if (claimValue == null)
                return null;

            if (!int.TryParse(claimValue, out int id))
                return null;

            // Carga el usuario con:
            // - Grupos
            // - PermisoCompuesto
            // - Hijos (Permisos simples)
            var usuario = _contexto.Personas
                .Include(u => u.Grupos)
                    .ThenInclude(g => g.Permisos)
                        .ThenInclude(pc => (pc as PermisoCompuesto).Permisos)
                .FirstOrDefault(u => u.Id == id);

            return usuario;
        }
    }
}
