using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Pago
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un contrato")]
        public int ContratoId { get; set; }

        public DateTime FechaPago { get; set; } = DateTime.UtcNow;

        [Range(0.01, 100000000, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio")]
        [StringLength(40)]
        public string Metodo { get; set; } = "Efectivo";

        [StringLength(100)]
        public string? Referencia { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Confirmado";

        public Contrato Contrato { get; set; } = null!;

        public ICollection<PagoDetalle> Detalles { get; set; } = new List<PagoDetalle>();
    }
}