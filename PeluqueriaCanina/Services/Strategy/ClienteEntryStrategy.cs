using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Models.Users;

public class ClienteEntryStrategy : IEntryStrategy
{
    public IActionResult Execute(Controller controller, Persona usuario)
    {
        return controller.RedirectToAction("Dashboard", "Cliente");
    }
}
