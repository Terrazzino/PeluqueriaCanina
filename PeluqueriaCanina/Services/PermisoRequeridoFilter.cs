using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PeluqueriaCanina.Services;

public class PermisoRequeridoFilter : IAuthorizationFilter
{
    private readonly string _permiso;
    private readonly IUsuarioActualService _usuarioService;

    public PermisoRequeridoFilter(string permiso, IUsuarioActualService usuarioService)
    {
        _permiso = permiso;
        _usuarioService = usuarioService;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var usuario = _usuarioService.Obtener();
        if (usuario == null)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        // Recorremos todos los grupos y sus permisos
        bool tienePermiso = usuario.Grupos.Any(g => g.Permisos.TienePermiso(_permiso));

        if (!tienePermiso)
        {
            context.Result = new RedirectToActionResult("AccesoDenegado", "Home", null);
        }
    }
}
