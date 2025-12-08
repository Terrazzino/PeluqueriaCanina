using System;

namespace PeluqueriaCanina.Models.ClasesDeAdministrador
{
    public class Auditoria
    {
        public int Id { get; set; }

        public int AdministradorId { get; set; }
        public string AdministradorNombre { get; set; }

        public int UsuarioModificadoId { get; set; }
        public string UsuarioModificadoNombre { get; set; }

        public string Accion { get; set; } // Ej: "Asignó Grupo", "Quitó Permiso", etc.
        public string Detalle { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
