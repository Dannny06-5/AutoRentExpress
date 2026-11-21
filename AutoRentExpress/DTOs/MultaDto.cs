using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class MultaDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un contrato")]
        public int ContratoId { get; set; }

        [Required(ErrorMessage = "El motivo de la multa es obligatorio")]
        [StringLength(100, ErrorMessage = "El motivo no puede superar los 100 caracteres")]
        public string Motivo { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "100000000",
            ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; }

        public bool Pagada { get; set; }

        public string NumeroContrato { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;
    }

    public class MultaContratoDto
    {
        public int Id { get; set; }

        public string NumeroContrato { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;
    }
}