using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class PagoDetalle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un pago")]
        public int PagoId { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [StringLength(100)]
        public string Concepto { get; set; } = string.Empty;

        [Range(0.01, 100000000, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Monto { get; set; }

        public Pago Pago { get; set; } = null!;
    }
}