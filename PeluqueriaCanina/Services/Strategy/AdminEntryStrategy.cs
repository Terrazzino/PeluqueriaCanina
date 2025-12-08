using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Models.Users;

public class AdminEntryStrategy : IEntryStrategy
{
    public IActionResult Execute(Controller controller, Persona usuario)
    {
        return controller.RedirectToAction("Dashboard", "Administrador");
    }
}
