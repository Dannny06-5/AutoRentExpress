using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class ModeloDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una marca")]
        public int MarcaId { get; set; }

        public string? NombreMarca { get; set; }
    }
}