using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class MantenimientoDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un vehículo")]
        public int VehiculoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un empleado")]
        public int EmpleadoId { get; set; }

        [Required(ErrorMessage = "El tipo de mantenimiento es obligatorio")]
        [StringLength(100)]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        [Range(typeof(decimal), "0", "100000000",
            ErrorMessage = "El costo debe estar entre 0 y 100000000")]
        public decimal CostoTotal { get; set; }

        public string Estado { get; set; } = "Pendiente";

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public string NombreVehiculo { get; set; } = string.Empty;

        public string PlacaVehiculo { get; set; } = string.Empty;

        public string NombreEmpleado { get; set; } = string.Empty;
    }

    public class MantenimientoOpcionDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }

    public class MantenimientoCatalogosDto
    {
        public List<MantenimientoOpcionDto> Vehiculos { get; set; } = new();

        public List<MantenimientoOpcionDto> Empleados { get; set; } = new();
    }
}