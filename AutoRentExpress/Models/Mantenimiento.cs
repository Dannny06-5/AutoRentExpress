using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Mantenimiento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un vehículo")]
        public int VehiculoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un empleado")]
        public int EmpleadoId { get; set; }

        [Required(ErrorMessage = "El tipo de mantenimiento es obligatorio")]
        [StringLength(100)]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        [Range(0, 100000000, ErrorMessage = "El costo no puede ser negativo")]
        public decimal CostoTotal { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public Vehiculo Vehiculo { get; set; } = null!;

        public Empleado Empleado { get; set; } = null!;

        public ICollection<MantenimientoDetalle> Detalles { get; set; } = new List<MantenimientoDetalle>();
    }
}