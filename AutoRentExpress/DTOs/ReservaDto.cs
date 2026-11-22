
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class ReservaDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un cliente")]
        public int ClienteId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un vehículo")]
        public int VehiculoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona la sucursal de retiro")]
        public int SucursalRetiroId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona la sucursal de devolución")]
        public int SucursalDevolucionId { get; set; }

        [Required(ErrorMessage = "Indica la fecha de inicio")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "Indica la fecha de devolución")]
        public DateTime FechaFin { get; set; }

        public int? SeguroId { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string Estado { get; set; } = "Pendiente";

        public decimal TotalEstimado { get; set; }

        public string NombreCliente { get; set; } = string.Empty;
        public string NombreVehiculo { get; set; } = string.Empty;
        public string PlacaVehiculo { get; set; } = string.Empty;
        public string NombreSucursalRetiro { get; set; } = string.Empty;
        public string NombreSucursalDevolucion { get; set; } = string.Empty;
        public string NombreSeguro { get; set; } = string.Empty;
        public decimal CostoSeguro { get; set; }
        public bool TieneContrato { get; set; }
        public int? TarifaId { get; set; }

    }

    public class ReservaOpcionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal TarifaDiaria { get; set; }
        public int CategoriaVehiculoId { get; set; }
    }

    public class ReservaCatalogosDto
    {
        public List<ReservaOpcionDto> Clientes { get; set; } = new();
        public List<ReservaOpcionDto> Vehiculos { get; set; } = new();
        public List<ReservaOpcionDto> Sucursales { get; set; } = new();
        public List<ReservaOpcionDto> Seguros { get; set; } = new();
        public List<ReservaTarifaOpcionDto> Tarifas { get; set; } = new();

    }
}
