using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class MantenimientoDetalle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un mantenimiento")]
        public int MantenimientoId { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(150)]
        public string Descripcion { get; set; } = string.Empty;

        [Range(1, 10000, ErrorMessage = "La cantidad debe ser mayor que cero")]
        public int Cantidad { get; set; } = 1;

        [Range(0, 100000000)]
        public decimal PrecioUnitario { get; set; }

        [Range(0, 100000000)]
        public decimal Subtotal { get; set; }

        public Mantenimiento Mantenimiento { get; set; } = null!;
    }
}