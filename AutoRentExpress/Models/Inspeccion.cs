using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Inspeccion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un contrato")]
        public int ContratoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un empleado")]
        public int EmpleadoId { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Debe seleccionar el tipo de inspección")]
        [StringLength(30)]
        public string Tipo { get; set; } = "Entrega";

        [Range(0, int.MaxValue)]
        public int Kilometraje { get; set; }

        [Range(0, 100, ErrorMessage = "El combustible debe estar entre 0 y 100")]
        public decimal NivelCombustible { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public Contrato Contrato { get; set; } = null!;

        public Empleado Empleado { get; set; } = null!;

        public ICollection<InspeccionDetalle> Detalles { get; set; } = new List<InspeccionDetalle>();
    }
}