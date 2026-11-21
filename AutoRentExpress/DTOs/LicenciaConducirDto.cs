
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class LicenciaConducirDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El número de licencia es obligatorio")]
        [StringLength(40)]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(20)]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona la fecha de emisión")]
        [DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "Selecciona la fecha de vencimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Required(ErrorMessage = "El país de emisión es obligatorio")]
        [StringLength(100)]
        public string PaisEmision { get; set; } = "Nicaragua";

        public string NombreCliente { get; set; } = string.Empty;
    }

    public class LicenciaClienteDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
