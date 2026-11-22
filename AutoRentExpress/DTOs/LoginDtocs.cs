
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Ingresa tu usuario.")]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresa tu contraseña.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;

        public bool Recordarme { get; set; }
    }
}
