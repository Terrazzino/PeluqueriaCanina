using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Models.Users;

public class PeluqueroEntryStrategy : IEntryStrategy
{
    public IActionResult Execute(Controller controller, Persona usuario)
    {
        return controller.RedirectToAction("Agenda", "Peluquero");
    }
}
