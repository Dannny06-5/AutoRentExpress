using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class SeguroDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del seguro es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La cobertura no puede superar los 500 caracteres")]
        public string? Cobertura { get; set; }

        [Range(typeof(decimal), "0", "1000000", ErrorMessage = "El costo diario debe estar entre 0 y 1000000")]
        public decimal CostoDiario { get; set; }

        public bool Activo { get; set; } = true;
    }
}