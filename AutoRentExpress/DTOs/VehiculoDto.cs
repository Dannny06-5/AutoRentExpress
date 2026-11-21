using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AutoRentExpress.DTOs
{
    public class VehiculoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La placa es obligatoria")]
        [StringLength(20)]
        public string Placa { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un modelo")]
        public int ModeloId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría")]
        public int CategoriaVehiculoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un estado")]
        public int EstadoVehiculoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una sucursal")]
        public int SucursalId { get; set; }

        [Required(ErrorMessage = "El color es obligatorio")]
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;

        [Range(1950, 2100, ErrorMessage = "Ingresa un año válido")]
        public int Anio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El kilometraje no puede ser negativo")]
        public int Kilometraje { get; set; }

        [Range(1, 100, ErrorMessage = "Ingresa una capacidad válida")]
        public int CapacidadPasajeros { get; set; }

        [Range(typeof(decimal), "0.01", "99999999", ErrorMessage = "La tarifa debe ser mayor que cero")]
        public decimal TarifaDiaria { get; set; }

        public string? ImagenUrl { get; set; }

        public IFormFile? ImagenArchivo { get; set; }

        public bool Activo { get; set; } = true;

        public string? NombreModelo { get; set; }
        public string? NombreCategoria { get; set; }
        public string? NombreEstado { get; set; }
        public string? NombreSucursal { get; set; }
    }
}