using System;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Multa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un contrato")]
        public int ContratoId { get; set; }

        [Required(ErrorMessage = "El motivo de la multa es obligatorio")]
        [StringLength(100)]
        public string Motivo { get; set; } = string.Empty;

        [Range(0.01, 100000000, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public bool Pagada { get; set; } = false;

        public Contrato Contrato { get; set; } = null!;
    }
}