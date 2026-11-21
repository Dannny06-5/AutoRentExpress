using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class CategoriaVehiculoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250)]
        public string Descripcion { get; set; } = string.Empty;
    }
}