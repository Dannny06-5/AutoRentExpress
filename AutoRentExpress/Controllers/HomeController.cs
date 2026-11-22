
using System.Diagnostics;
using AutoRentExpress.Data;
using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            AppDbContext context,
            ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
            var inicioPeriodo = inicioMes.AddMonths(-5);
            var finPeriodo = inicioMes.AddMonths(1);

            var vehiculos = await _context.Set<Vehiculo>()
                .AsNoTracking()
                .Include(v => v.EstadoVehiculo)
                .ToListAsync();

            var reservas = await _context.Set<Reserva>()
                .AsNoTracking()
                .ToListAsync();

            var contratos = await _context.Set<Contrato>()
                .AsNoTracking()
                .ToListAsync();

            var pagos = await _context.Set<Pago>()
                .AsNoTracking()
                .Where(p => p.Estado == "Confirmado")
                .ToListAsync();

            var licenciasVencidas = await _context
                .Set<LicenciaConducir>()
                .AsNoTracking()
                .CountAsync(l => l.FechaVencimiento < hoy);

            var mantenimientosEnProceso = await _context
                .Set<Mantenimiento>()
                .AsNoTracking()
                .CountAsync(m => m.Estado == "En proceso");

            var ultimosContratos = await _context.Set<Contrato>()
                .AsNoTracking()
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Cliente)
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .OrderByDescending(c => c.FechaFirma)
                .ThenByDescending(c => c.Id)
                .Take(5)
                .ToListAsync();

            var ultimasReservas = await _context.Set<Reserva>()
                .AsNoTracking()
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .OrderByDescending(r => r.FechaRegistro)
                .ThenByDescending(r => r.Id)
                .Take(5)
                .ToListAsync();

            var totalPagadoPorContrato = pagos
                .GroupBy(p => p.ContratoId)
                .ToDictionary(
                    grupo => grupo.Key,
                    grupo => grupo.Sum(p => p.Monto)
                );

            var saldoPendiente = contratos
                .Where(c =>
                    c.Estado == "Pendiente de entrega" ||
                    c.Estado == "En curso" ||
                    c.Estado == "Finalizado")
                .Sum(c =>
                {
                    var pagado = totalPagadoPorContrato.GetValueOrDefault(c.Id);

                    return Math.Max(0m, c.TotalContrato - pagado);
                });

            var ingresosMensuales = new List<DashboardIngresoDto>();

            var nombresMeses = new[]
            {
                "Ene", "Feb", "Mar", "Abr", "May", "Jun",
                "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"
            };

            for (int i = 0; i < 6; i++)
            {
                var mes = inicioPeriodo.AddMonths(i);
                var siguienteMes = mes.AddMonths(1);

                var totalMes = pagos
                    .Where(p =>
                        p.FechaPago >= mes &&
                        p.FechaPago < siguienteMes)
                    .Sum(p => p.Monto);

                ingresosMensuales.Add(new DashboardIngresoDto
                {
                    Mes = $"{nombresMeses[mes.Month - 1]} {mes.Year}",
                    Total = totalMes
                });
            }

            var modelo = new DashboardDto
            {
                TotalVehiculos = vehiculos.Count,

                VehiculosDisponibles = vehiculos.Count(v =>
                    v.Activo &&
                    NormalizarEstado(v.EstadoVehiculo?.Nombre) == "DISPONIBLE"),

                VehiculosAlquilados = vehiculos.Count(v =>
                    v.Activo &&
                    NormalizarEstado(v.EstadoVehiculo?.Nombre) == "ALQUILADO"),

                VehiculosMantenimiento = vehiculos.Count(v =>
                    v.Activo &&
                    (NormalizarEstado(v.EstadoVehiculo?.Nombre) == "MANTENIMIENTO" ||
                     NormalizarEstado(v.EstadoVehiculo?.Nombre) == "EN MANTENIMIENTO")),

                TotalReservas = reservas.Count,

                ReservasPendientes = reservas.Count(r =>
                    r.Estado == "Pendiente"),

                ReservasEnCurso = reservas.Count(r =>
                    r.Estado == "En curso"),

                ReservasFinalizadas = reservas.Count(r =>
                    r.Estado == "Finalizada"),

                TotalContratos = contratos.Count,

                ContratosPendientes = contratos.Count(c =>
                    c.Estado == "Pendiente de entrega"),

                ContratosEnCurso = contratos.Count(c =>
                    c.Estado == "En curso"),

                ContratosFinalizados = contratos.Count(c =>
                    c.Estado == "Finalizado"),

                LicenciasVencidas = licenciasVencidas,
                MantenimientosEnProceso = mantenimientosEnProceso,

                TotalIngresos = pagos.Sum(p => p.Monto),

                IngresosMes = pagos
                    .Where(p =>
                        p.FechaPago >= inicioMes &&
                        p.FechaPago < finPeriodo)
                    .Sum(p => p.Monto),

                SaldoPendiente = saldoPendiente,

                IngresosMensuales = ingresosMensuales,

                UltimosContratos = ultimosContratos
                    .Select(c => new DashboardContratoDto
                    {
                        Id = c.Id,
                        Numero = c.NumeroContrato,

                        Cliente = c.Reserva?.Cliente == null
                            ? "Sin cliente"
                            : $"{c.Reserva.Cliente.Nombres} " +
                              $"{c.Reserva.Cliente.Apellidos}",

                        Vehiculo = c.Reserva?.Vehiculo == null
                            ? "Sin vehículo"
                            : $"{c.Reserva.Vehiculo.Modelo?.Nombre} " +
                              $"({c.Reserva.Vehiculo.Placa})",

                        Fecha = c.FechaFirma,
                        Total = c.TotalContrato,
                        Estado = c.Estado
                    })
                    .ToList(),

                UltimasReservas = ultimasReservas
                    .Select(r => new DashboardReservaDto
                    {
                        Id = r.Id,

                        Cliente = r.Cliente == null
                            ? "Sin cliente"
                            : $"{r.Cliente.Nombres} {r.Cliente.Apellidos}",

                        Vehiculo = r.Vehiculo == null
                            ? "Sin vehículo"
                            : $"{r.Vehiculo.Modelo?.Nombre} " +
                              $"({r.Vehiculo.Placa})",

                        FechaInicio = r.FechaInicio,
                        Total = r.TotalEstimado,
                        Estado = r.Estado
                    })
                    .ToList()
            };

            return View(modelo);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }

        private static string NormalizarEstado(string? estado)
        {
            return (estado ?? string.Empty)
                .Trim()
                .TrimEnd('.')
                .Trim()
                .ToUpperInvariant();
        }
    }
}
