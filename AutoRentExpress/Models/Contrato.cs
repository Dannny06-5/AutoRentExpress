using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Contrato
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una reserva")]
        public int ReservaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un empleado")]
        public int EmpleadoId { get; set; }

        [Required(ErrorMessage = "El número de contrato es obligatorio")]
        [StringLength(30)]
        public string NumeroContrato { get; set; } = string.Empty;

        public DateTime FechaFirma { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime FechaEntrega { get; set; }

        [Required]
        public DateTime FechaDevolucionPrevista { get; set; }

        public DateTime? FechaDevolucionReal { get; set; }

        [Range(0, 100000000)]
        public decimal TotalContrato { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Activo";

        public Reserva Reserva { get; set; } = null!;

        public Empleado Empleado { get; set; } = null!;

        public ICollection<ContratoDetalle> Detalles { get; set; } = new List<ContratoDetalle>();

        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();

        public ICollection<Multa> Multas { get; set; } = new List<Multa>();

        public ICollection<Inspeccion> Inspecciones { get; set; } = new List<Inspeccion>();
    }
}