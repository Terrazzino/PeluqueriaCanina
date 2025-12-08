using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.Users;
using PeluqueriaCanina.Models.ViewModels;
using PeluqueriaCanina.Services;

public class PerfilController : Controller
{
    private readonly ContextoAcqua _contexto;
    private readonly IUsuarioActualService _usuarioActual;

    public PerfilController(ContextoAcqua contexto, IUsuarioActualService usuarioActual)
    {
        _contexto = contexto;
        _usuarioActual = usuarioActual;
    }

    // GET PERFIL
    [HttpGet]
    public IActionResult MiPerfil()
    {
        var usuario = _usuarioActual.Obtener();
        if (usuario == null)
            return RedirectToAction("Login", "Auth");

        var vm = new EditarPersonaViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Dni = usuario.Dni,
            FechaDeNacimiento = usuario.FechaDeNacimiento
        };

        // Enviamos la ruta dinámica a la vista
        ViewBag.DashboardUrl = ObtenerDashboardSegunTipo(usuario);

        return View(vm);
    }


    // POST PERFIL
    [HttpPost]
    public IActionResult MiPerfil(EditarPersonaViewModel vm, string nuevaContraseña)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var persona = _contexto.Personas.FirstOrDefault(p => p.Id == vm.Id);
        if (persona == null)
            return RedirectToAction("Login", "Auth");

        persona.Nombre = vm.Nombre;
        persona.Apellido = vm.Apellido;
        persona.Dni = vm.Dni;
        persona.FechaDeNacimiento = vm.FechaDeNacimiento;

        if (!string.IsNullOrWhiteSpace(nuevaContraseña))
            persona.RegistrarContraseña(nuevaContraseña);

        _contexto.SaveChanges();

        ViewBag.Ok = "Datos actualizados correctamente.";
        ViewBag.DashboardUrl = ObtenerDashboardSegunTipo(persona);
        return View(vm);

    }

    private string ObtenerDashboardSegunTipo(Persona persona)
    {
        return persona switch
        {
            Administrador => Url.Action("Dashboard", "Administrador"),
            Cliente => Url.Action("Dashboard", "Cliente"),
            Peluquero => Url.Action("Agenda", "Peluquero"),
            _ => Url.Action("Login", "Auth")
        };
    }

}
