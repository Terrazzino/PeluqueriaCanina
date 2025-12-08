using Microsoft.AspNetCore.Mvc;
using PeluqueriaCanina.Models.Users;

public interface IEntryStrategy
{
    IActionResult Execute(Controller controller, Persona usuario);
}
