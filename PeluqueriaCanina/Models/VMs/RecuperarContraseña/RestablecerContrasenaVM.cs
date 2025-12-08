using System.ComponentModel.DataAnnotations;

public class RestablecerContrasenaVM
{
    [Required, EmailAddress]
    public string Mail { get; set; }

    [Required]
    public string Token { get; set; }

    [Required, MinLength(8)]
    public string NuevaContrasena { get; set; }

    [Required, Compare("NuevaContrasena")]
    public string ConfirmarContrasena { get; set; }
}
