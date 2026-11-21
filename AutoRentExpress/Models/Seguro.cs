using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class Seguro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del seguro es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Cobertura { get; set; }

        [Range(0, 1000000, ErrorMessage = "El costo no puede ser negativo")]
        public decimal CostoDiario { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<ReservaDetalle> ReservaDetalles { get; set; } = new List<ReservaDetalle>();
    }
}