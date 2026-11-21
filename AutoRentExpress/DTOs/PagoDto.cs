using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class PagoDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un contrato")]
        public int ContratoId { get; set; }

        public DateTime FechaPago { get; set; }

        [Range(typeof(decimal), "0.01", "100000000",
            ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "Selecciona el método de pago")]
        [StringLength(40)]
        public string Metodo { get; set; } = "Efectivo";

        [StringLength(100)]
        public string? Referencia { get; set; }

        public string Estado { get; set; } = "Confirmado";

        public string NumeroContrato { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;

        public decimal TotalContrato { get; set; }

        public decimal TotalPagado { get; set; }

        public decimal SaldoPendiente { get; set; }
    }

    public class PagoContratoDto
    {
        public int Id { get; set; }

        public string NumeroContrato { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;

        public decimal TotalContrato { get; set; }

        public decimal TotalPagado { get; set; }

        public decimal SaldoPendiente { get; set; }
    }
}