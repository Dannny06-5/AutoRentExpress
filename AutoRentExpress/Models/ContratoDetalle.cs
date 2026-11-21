using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class ContratoDetalle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un contrato")]
        public int ContratoId { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [StringLength(100)]
        public string Concepto { get; set; } = string.Empty;

        [Range(1, 10000, ErrorMessage = "La cantidad debe ser mayor que cero")]
        public int Cantidad { get; set; } = 1;

        [Range(0, 100000000)]
        public decimal PrecioUnitario { get; set; }

        [Range(0, 100000000)]
        public decimal Subtotal { get; set; }

        public Contrato Contrato { get; set; } = null!;
    }
}