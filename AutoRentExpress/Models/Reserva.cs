using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace AutoRentExpress.Models
{
    public class Reserva
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un vehículo")]
        public int VehiculoId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una sucursal de retiro")]
        public int SucursalRetiroId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una sucursal de devolución")]
        public int SucursalDevolucionId { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es obligatoria")]
        public DateTime FechaFin { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Pendiente";

        [Range(0, 100000000)]
        public decimal TotalEstimado { get; set; }

        public Cliente Cliente { get; set; } = null!;

        public Vehiculo Vehiculo { get; set; } = null!;

        public Sucursal SucursalRetiro { get; set; } = null!;

        public Sucursal SucursalDevolucion { get; set; } = null!;

        public ICollection<ReservaDetalle> Detalles { get; set; } = new List<ReservaDetalle>();

        public Contrato? Contrato { get; set; }
    }
}