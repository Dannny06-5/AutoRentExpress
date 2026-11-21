using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class MarcaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }
}