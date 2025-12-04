using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Models.Permisos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PeluqueriaCanina.Models.Users
{
    public abstract class Usuario
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Mail { get; set; } = string.Empty;
        [Required]
        public string ContraseñaHasheada { get; set; } = string.Empty;
        [Required]
        public string Rol { get; set; } = "Administrador";
        [NotMapped]
        public Permiso Permisos { get; set; }

        public void RegistrarContraseña(string contraseña)
        {
            if (string.IsNullOrWhiteSpace(contraseña) || contraseña.Length < 8)
                throw new ArgumentException("La contraseña debe tener minimo 8 caracteres");
            ContraseñaHasheada = BCrypt.Net.BCrypt.HashPassword(contraseña);
        }
        public bool VerificarContraseña(string contraseña)
        {
            return BCrypt.Net.BCrypt.Verify(contraseña, ContraseñaHasheada);
        }

        public bool TienePermiso(string accion)
        {
            return Permisos?.TienePermiso(accion) ?? false;
        }

    }
}
