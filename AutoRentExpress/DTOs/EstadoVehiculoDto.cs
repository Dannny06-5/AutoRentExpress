using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class EstadoVehiculoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del estado es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}