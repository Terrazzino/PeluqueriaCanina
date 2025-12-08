using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Models.Factories;
using PeluqueriaCanina.Models.Grupos;

public class GrupoController : Controller
{
    private readonly ContextoAcqua _context;

    public GrupoController(ContextoAcqua context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(_context.Grupos.ToList());
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Crear(string nombre)
    {
        var grupo = new Grupo
        {
            Nombre = nombre,
            Permisos = new PermisoCompuesto
            {
                Nombre = $"Permisos_{nombre}"
            }
        };

        _context.Grupos.Add(grupo);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }


    public IActionResult Editar(int id)
    {
        var grupo = _context.Grupos.FirstOrDefault(g => g.Id == id);
        return View(grupo);
    }

    [HttpPost]
    public IActionResult Editar(int id, string nombre)
    {
        var grupo = _context.Grupos.FirstOrDefault(g => g.Id == id);
        grupo.Nombre = nombre;
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Eliminar(int id)
    {
        var grupo = _context.Grupos.FirstOrDefault(g => g.Id == id);
        _context.Grupos.Remove(grupo);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult AsignarPermisos(int id)
    {
        var grupo = _context.Grupos
            .Include(g => g.Permisos)
            .ThenInclude(pc => pc.Permisos)
            .FirstOrDefault(g => g.Id == id);

        if (grupo == null)
            return NotFound();

        // 1) Obtener todos los permisos posibles
        var todosLosPermisos = PermisoFactory
            .CrearPermiso("Administrador")
            .ListarPermisos()
            .Concat(PermisoFactory.CrearPermiso("Cliente").ListarPermisos())
            .Concat(PermisoFactory.CrearPermiso("Peluquero").ListarPermisos())
            .Distinct()
            .ToList();

        // 2) Crear ViewModel
        var vm = new AsignarPermisosViewModel
        {
            GrupoId = grupo.Id,
            NombreGrupo = grupo.Nombre,
            PermisosDisponibles = todosLosPermisos,
            PermisosAsignados = grupo.Permisos.ListarPermisos()
        };

        return View(vm);
    }

    [HttpPost]
    public IActionResult AsignarPermisos(int id, List<string> permisosSeleccionados)
    {
        var grupo = _context.Grupos
            .Include(g => g.Permisos)
            .ThenInclude(pc => pc.Permisos)
            .FirstOrDefault(g => g.Id == id);

        if (grupo == null)
            return NotFound();

        // 1) Vaciar permisos actuales
        grupo.Permisos.Permisos.Clear();

        // 2) Agregar nuevos permisos (simples) según los checks seleccionados
        foreach (var p in permisosSeleccionados)
        {
            grupo.Permisos.AgregarPermiso(new PermisoSimple { Nombre = p });
        }

        _context.SaveChanges();

        return RedirectToAction("Index");
    }



}
