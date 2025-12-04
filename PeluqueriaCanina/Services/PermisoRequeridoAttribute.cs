using Microsoft.AspNetCore.Mvc;

public class PermisoRequeridoAttribute : TypeFilterAttribute
{
    public PermisoRequeridoAttribute(string permiso)
        : base(typeof(PermisoRequeridoFilter))
    {
        Arguments = new object[] { permiso };
    }
}
