using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.DTOs
{
    public class ContratoDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una reserva")]
        public int ReservaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un empleado")]
        public int EmpleadoId { get; set; }

        public string NumeroContrato { get; set; } = string.Empty;

        public DateTime FechaFirma { get; set; }

        public DateTime FechaEntrega { get; set; }

        public DateTime FechaDevolucionPrevista { get; set; }

        public DateTime? FechaDevolucionReal { get; set; }

        public decimal TotalContrato { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;

        public string Vehiculo { get; set; } = string.Empty;

        public string PlacaVehiculo { get; set; } = string.Empty;
    }

    public class ContratoOpcionDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }

    public class ContratoCatalogosDto
    {
        public List<ContratoOpcionDto> Reservas { get; set; } = new();

        public List<ContratoOpcionDto> Empleados { get; set; } = new();
    }
}