using System;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Tarifa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una categoría")]
        public int CategoriaVehiculoId { get; set; }

        [Required(ErrorMessage = "El nombre de la tarifa es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Range(0.01, 1000000, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal PrecioDiario { get; set; }

        [DataType(DataType.Date)]
        public DateTime VigenteDesde { get; set; }

        [DataType(DataType.Date)]
        public DateTime? VigenteHasta { get; set; }

        public bool Activa { get; set; } = true;

        public CategoriaVehiculo CategoriaVehiculo { get; set; } = null!;
    }
}