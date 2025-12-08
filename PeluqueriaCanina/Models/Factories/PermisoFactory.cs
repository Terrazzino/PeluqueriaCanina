namespace PeluqueriaCanina.Models.Factories
{
    public class PermisoFactory
    {
        public static Permiso CrearPermiso(string rol)
        {
            switch(rol)
            {
                case "Cliente":
                    var clientePermisos = new PermisoCompuesto { Nombre = "PermisosCliente" };
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "AccederDashboardCliente" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "RegistrarMascota" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "EliminarMascota" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "ModificarMascota" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerMascotas" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "RegistrarTurno" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "CancelarTurno" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "ModificarTurno" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerTurnos" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerDisponibilidadTurnos" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "RealizarPago" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "MisPagos" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "RegistrarPuntuacionPeluquero" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "DashboardVeterinaria" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "ReservarTurnoVeterinaria" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "CancelarTurnoVeterinaria" });
                    clientePermisos.AgregarPermiso(new PermisoSimple { Nombre = "MisTurnosVeterinaria" });
                    return clientePermisos;
                case "Peluquero":
                    var peluqueroPermisos = new PermisoCompuesto { Nombre = "PermisosPeluquero" };
                    peluqueroPermisos.AgregarPermiso(new PermisoSimple { Nombre = "AccederDashboardPeluquero" });
                    peluqueroPermisos.AgregarPermiso(new PermisoSimple { Nombre = "CambiarEstadoTurno" });
                    peluqueroPermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerTurnos" });
                    peluqueroPermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerValoracionDePeluqueros" });
                    return peluqueroPermisos;
                case "Administrador":
                    var administradorPermisos = new PermisoCompuesto { Nombre = "PermisosAdministrador" };
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "AccederDashboardAdministrador" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "RegistrarPeluquero" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "EliminarPeluquero" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "ModificarPeluquero" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerPeluqueros" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "RegistrarServicio" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "EliminarServicio" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "ModificarServicio" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerServicios" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerValoracionDePeluqueros" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "VerReporte" });
                    administradorPermisos.AgregarPermiso(new PermisoSimple { Nombre = "ListarPuntuacionPeluquero" });
                    return administradorPermisos;
                default:
                    throw new ArgumentException("Rol no identificado");
            }
        }
    }
}
