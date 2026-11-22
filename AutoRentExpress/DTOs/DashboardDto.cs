
namespace AutoRentExpress.DTOs
{
    public class DashboardDto
    {
        public int TotalVehiculos { get; set; }
        public int VehiculosDisponibles { get; set; }
        public int VehiculosAlquilados { get; set; }
        public int VehiculosMantenimiento { get; set; }

        public int TotalReservas { get; set; }
        public int ReservasPendientes { get; set; }
        public int ReservasEnCurso { get; set; }
        public int ReservasFinalizadas { get; set; }

        public int TotalContratos { get; set; }
        public int ContratosPendientes { get; set; }
        public int ContratosEnCurso { get; set; }
        public int ContratosFinalizados { get; set; }

        public int LicenciasVencidas { get; set; }
        public int MantenimientosEnProceso { get; set; }

        public decimal TotalIngresos { get; set; }
        public decimal IngresosMes { get; set; }
        public decimal SaldoPendiente { get; set; }

        public List<DashboardIngresoDto> IngresosMensuales { get; set; } = new();
        public List<DashboardContratoDto> UltimosContratos { get; set; } = new();
        public List<DashboardReservaDto> UltimasReservas { get; set; } = new();
    }

    public class DashboardIngresoDto
    {
        public string Mes { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class DashboardContratoDto
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string Vehiculo { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class DashboardReservaDto
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Vehiculo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
