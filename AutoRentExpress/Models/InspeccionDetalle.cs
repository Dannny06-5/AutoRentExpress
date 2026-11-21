using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class InspeccionDetalle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una inspección")]
        public int InspeccionId { get; set; }

        [Required(ErrorMessage = "El elemento es obligatorio")]
        [StringLength(100)]
        public string Elemento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es obligatorio")]
        [StringLength(50)]
        public string Estado { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Observacion { get; set; }

        [Range(0, 100000000)]
        public decimal CargoAdicional { get; set; }

        public Inspeccion Inspeccion { get; set; } = null!;
    }
}