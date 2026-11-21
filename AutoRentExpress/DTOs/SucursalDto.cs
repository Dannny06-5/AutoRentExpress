using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class SucursalDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(250)]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(30)]
        public string? Telefono { get; set; }

        public bool Activa { get; set; } = true;
    }
}