using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class InspeccionDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un contrato")]
        public int ContratoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un empleado")]
        public int EmpleadoId { get; set; }

        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "Selecciona el tipo de inspección")]
        [StringLength(30)]
        public string Tipo { get; set; } = "Entrega";

        [Range(0, int.MaxValue, ErrorMessage = "El kilometraje no puede ser negativo")]
        public int Kilometraje { get; set; }

        [Range(typeof(decimal), "0", "100", ErrorMessage = "El combustible debe estar entre 0 y 100")]
        public decimal NivelCombustible { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden superar los 500 caracteres")]
        public string? Observaciones { get; set; }

        public string NumeroContrato { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;

        public string NombreVehiculo { get; set; } = string.Empty;

        public string NombreEmpleado { get; set; } = string.Empty;
    }

    public class InspeccionOpcionDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }

    public class InspeccionCatalogosDto
    {
        public List<InspeccionOpcionDto> Contratos { get; set; } = new();

        public List<InspeccionOpcionDto> Empleados { get; set; } = new();
    }
}