namespace PeluqueriaCanina.Models.ClasesDeTurno
{
    public class EstadoPendiente : IEstadoTurno
    {
        public string Nombre => "Pendiente";
        public bool PuedeModificar => true;
        public bool PuedeCancelar => true;
        public bool PuedeValorar => false;
    }

    public class EstadoPendientePago : IEstadoTurno
    {
        public string Nombre => "Pendiente de Pago";
        public bool PuedeModificar => true;
        public bool PuedeCancelar => true;
        public bool PuedeValorar => false;
    }

    public class EstadoConfirmado : IEstadoTurno
    {
        public string Nombre => "Confirmado";
        public bool PuedeModificar => true;
        public bool PuedeCancelar => true;
        public bool PuedeValorar => false;
    }

    public class EstadoCompletado : IEstadoTurno
    {
        public string Nombre => "Completado";
        public bool PuedeModificar => false;
        public bool PuedeCancelar => false;
        public bool PuedeValorar => true;
    }

    public class EstadoCancelado : IEstadoTurno
    {
        public string Nombre => "Cancelado";
        public bool PuedeModificar => false;
        public bool PuedeCancelar => false;
        public bool PuedeValorar => false;
    }
}
