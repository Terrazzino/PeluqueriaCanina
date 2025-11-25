using System;

namespace AcquaDiCane.Models.DTOs
{
    public class ClienteVetDTO
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
    }

    public class MascotaVetDTO
    {
        public string Nombre { get; set; }
        public string Raza { get; set; }
        public double Peso { get; set; }
    }

    public class TurnoCreateDto
    {
        public ClienteVetDTO Cliente { get; set; }
        public MascotaVetDTO Mascota { get; set; }
        public DateTime FechaHora { get; set; }
    }

    public class TurnoResponseDto
    {
        public int Id { get; set; }
        public ClienteVetDTO Cliente { get; set; }
        public MascotaVetDTO Mascota { get; set; }
        public DateTime FechaHora { get; set; }
        public string Estado { get; set; }
    }
}
