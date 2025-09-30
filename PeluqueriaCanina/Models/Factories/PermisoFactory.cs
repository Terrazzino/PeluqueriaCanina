using PeluqueriaCanina.Models.Permisos;

namespace PeluqueriaCanina.Models.Factories
{
    public class PermisoFactory
    {
        public static Permiso CrearPermiso(string rol)
        {
            switch(rol)
            {
                case "Cliente":
                    var clientePermisos = new PermisoCompuesto();
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "RegistrarMascota" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "EliminarMascota" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "ModificarMascota" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerMascotas" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "RegistrarTurno" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "CancelarTurno" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "ModificarTurno" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerTurnos" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "RealizarPago" });
                    return clientePermisos;
                case "Peluquero":
                    var peluqueroPermisos = new PermisoCompuesto();
                    peluqueroPermisos.AgregarPermiso(new PermisoSimple { Nombre = "CancelarTurno" });
                    peluqueroPermisos.AgregarPermiso(new PermisoSimple { Nombre = "NotificarTurnoFinalizado" });
                    peluqueroPermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerTurnos" });
                    return peluqueroPermisos;
                case "Administrador":
                    var administradorPermisos = new PermisoCompuesto();
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "RegistrarPeluquero" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "EliminarPeluquero" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "ModificarPeluquero" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerPeluqueros" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "RegistrarServicio" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "EliminarServicio" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "ModificarServicio" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerServicios" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "GenerarReporte" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerReporte" });
                    return administradorPermisos;
                default:
                    throw new ArgumentException("Rol no identificado");
            }
        }
    }
}
