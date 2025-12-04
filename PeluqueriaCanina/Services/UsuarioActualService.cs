using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.Factories;
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
            var idString = _http.HttpContext.Session.GetString("UsuarioId");
            if (idString == null) return null;

            int id = int.Parse(idString);
            var usuario = _contexto.Personas.FirstOrDefault(u => u.Id == id);

            usuario.Permisos = PermisoFactory.CrearPermiso(usuario.Rol);
            return usuario;
        }
    }

}
