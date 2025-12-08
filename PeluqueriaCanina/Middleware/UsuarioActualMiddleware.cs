using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PeluqueriaCanina.Services;

public class UsuarioActualMiddleware
{
    private readonly RequestDelegate _next;

    public UsuarioActualMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUsuarioActualService usuarioService)
    {
        var usuario = usuarioService.Obtener();
        context.Items["UsuarioActual"] = usuario;

        await _next(context);
    }
}
