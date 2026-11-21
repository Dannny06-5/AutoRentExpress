using System;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Models
{
    public class LicenciaConducir
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El número de licencia es obligatorio")]
        [StringLength(40)]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(20)]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de emisión es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [Required(ErrorMessage = "El país de emisión es obligatorio")]
        [StringLength(100)]
        public string PaisEmision { get; set; } = "Nicaragua";

        public Cliente Cliente { get; set; } = null!;
    }
}