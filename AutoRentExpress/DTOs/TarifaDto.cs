
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class TarifaDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría")]
        public int CategoriaVehiculoId { get; set; }

        [Required(ErrorMessage = "El nombre de la tarifa es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "1000000",
            ErrorMessage = "El precio diario debe ser mayor que cero")]
        public decimal PrecioDiario { get; set; }

        [Required(ErrorMessage = "Selecciona la fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime VigenteDesde { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        public DateTime? VigenteHasta { get; set; }

        public bool Activa { get; set; } = true;

        public string NombreCategoria { get; set; } = string.Empty;
    }

    public class TarifaCategoriaDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
