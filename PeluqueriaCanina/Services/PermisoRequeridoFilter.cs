using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PeluqueriaCanina.Models;
using PeluqueriaCanina.Services;

public class PermisoRequeridoFilter : IAuthorizationFilter
{
    private readonly string _permiso;
    private readonly IUsuarioActualService _usuarioActual;

    public PermisoRequeridoFilter(string permiso, IUsuarioActualService usuarioActual)
    {
        _permiso = permiso;
        _usuarioActual = usuarioActual;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var usuario = _usuarioActual.Obtener();

        if (usuario == null || !usuario.Permisos.TienePermiso(_permiso))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }
    }
}
