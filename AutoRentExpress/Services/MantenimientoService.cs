
using AutoRentExpress.Data;
using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Services
{
    public class MantenimientoService : IMantenimientoService
    {
        private readonly IMantenimientoRepository _repository;
        private readonly AppDbContext _context;

        public MantenimientoService(
            IMantenimientoRepository repository,
            AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<List<MantenimientoDto>> ObtenerTodosAsync()
        {
            var mantenimientos = await _repository.ObtenerTodosAsync();

            return mantenimientos.Select(Mapear).ToList();
        }

        public async Task<MantenimientoDto?> ObtenerPorIdAsync(int id)
        {
            var mantenimiento = await _repository.ObtenerPorIdAsync(id);

            return mantenimiento == null ? null : Mapear(mantenimiento);
        }

        public async Task<MantenimientoCatalogosDto> ObtenerCatalogosAsync()
        {
            var vehiculos = await _repository.ObtenerVehiculosAsync();
            var empleados = await _repository.ObtenerEmpleadosAsync();

            return new MantenimientoCatalogosDto
            {
                Vehiculos = vehiculos
                    .Where(v => v.Activo)
                    .Select(v => new MantenimientoOpcionDto
                    {
                        Id = v.Id,
                        Nombre = $"{v.Placa} - {v.Modelo?.Nombre}"
                    })
                    .ToList(),

                Empleados = empleados
                    .Where(e => e.Activo)
                    .Select(e => new MantenimientoOpcionDto
                    {
                        Id = e.Id,
                        Nombre = $"{e.Nombres} {e.Apellidos} - {e.Cargo}"
                    })
                    .ToList()
            };
        }

        public async Task CrearAsync(MantenimientoDto dto)
        {
            await ValidarDatosAsync(dto);

            var mantenimiento = new Mantenimiento
            {
                VehiculoId = dto.VehiculoId,
                EmpleadoId = dto.EmpleadoId,
                Tipo = dto.Tipo.Trim(),
                FechaInicio = dto.FechaInicio,
                FechaFin = null,
                CostoTotal = dto.CostoTotal,
                Estado = "Pendiente",
                Observaciones = Normalizar(dto.Observaciones)
            };

            await _repository.AgregarAsync(mantenimiento);
        }

        public async Task ActualizarAsync(MantenimientoDto dto)
        {
            var mantenimiento = await _repository.ObtenerPorIdAsync(dto.Id);

            if (mantenimiento == null)
            {
                throw new KeyNotFoundException(
                    "El mantenimiento seleccionado no existe.");
            }

            if (mantenimiento.Estado == "Finalizado")
            {
                throw new InvalidOperationException(
                    "No puedes editar un mantenimiento finalizado.");
            }

            if (mantenimiento.Estado == "En proceso" &&
                mantenimiento.VehiculoId != dto.VehiculoId)
            {
                throw new InvalidOperationException(
                    "No puedes cambiar el vehículo de un mantenimiento en proceso.");
            }

            await ValidarDatosAsync(dto, dto.Id);

            mantenimiento.VehiculoId = dto.VehiculoId;
            mantenimiento.EmpleadoId = dto.EmpleadoId;
            mantenimiento.Tipo = dto.Tipo.Trim();
            mantenimiento.CostoTotal = dto.CostoTotal;
            mantenimiento.Observaciones = Normalizar(dto.Observaciones);

            if (mantenimiento.Estado == "Pendiente")
            {
                mantenimiento.FechaInicio = dto.FechaInicio;
            }

            await _repository.GuardarCambiosAsync();
        }

        public async Task IniciarAsync(int id)
        {
            var mantenimiento = await _context.Set<Mantenimiento>()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mantenimiento == null)
            {
                throw new KeyNotFoundException(
                    "El mantenimiento seleccionado no existe.");
            }

            if (mantenimiento.Estado != "Pendiente")
            {
                throw new InvalidOperationException(
                    "Solo puedes iniciar mantenimientos pendientes.");
            }

            var vehiculo = await _context.Set<Vehiculo>()
                .FirstOrDefaultAsync(v => v.Id == mantenimiento.VehiculoId);

            if (vehiculo == null || !vehiculo.Activo)
            {
                throw new InvalidOperationException(
                    "El vehículo no existe o se encuentra inactivo.");
            }

            var empleado = await _repository.ObtenerEmpleadoAsync(
                mantenimiento.EmpleadoId);

            if (empleado == null || !empleado.Activo)
            {
                throw new InvalidOperationException(
                    "El empleado responsable no existe o está inactivo.");
            }

            if (await _repository.ExisteMantenimientoActivoAsync(
                mantenimiento.VehiculoId,
                mantenimiento.Id))
            {
                throw new InvalidOperationException(
                    "El vehículo ya tiene otro mantenimiento en proceso.");
            }

            var estados = await _context.Set<EstadoVehiculo>()
                .AsNoTracking()
                .ToListAsync();

            var estadoDisponible = estados.FirstOrDefault(e =>
                NormalizarEstado(e.Nombre) == "DISPONIBLE");

            var estadoMantenimiento = estados.FirstOrDefault(e =>
                NormalizarEstado(e.Nombre) == "EN MANTENIMIENTO" ||
                NormalizarEstado(e.Nombre) == "MANTENIMIENTO");

            if (estadoDisponible == null)
            {
                throw new InvalidOperationException(
                    "No existe el estado Disponible en el catálogo de vehículos.");
            }

            if (estadoMantenimiento == null)
            {
                throw new InvalidOperationException(
                    "No existe el estado En mantenimiento o Mantenimiento en el catálogo de vehículos.");
            }

            var tieneContratoActivo = await _context.Set<Contrato>()
                .AsNoTracking()
                .AnyAsync(c =>
                    c.Reserva.VehiculoId == vehiculo.Id &&
                    (c.Estado == "En curso" ||
                     c.Estado == "Pendiente de entrega"));

            if (tieneContratoActivo)
            {
                throw new InvalidOperationException(
                    "No puedes iniciar el mantenimiento porque el vehículo tiene un contrato en curso o pendiente de entrega.");
            }

            var tieneReservasPendientes = await _context.Set<Reserva>()
                .AsNoTracking()
                .AnyAsync(r =>
                    r.VehiculoId == vehiculo.Id &&
                    (r.Estado == "Pendiente" ||
                     r.Estado == "Confirmada") &&
                    r.FechaFin > DateTime.Now);

            if (tieneReservasPendientes)
            {
                throw new InvalidOperationException(
                    "No puedes iniciar el mantenimiento porque el vehículo tiene reservas pendientes o confirmadas que todavía no han terminado.");
            }

            if (vehiculo.EstadoVehiculoId != estadoDisponible.Id)
            {
                throw new InvalidOperationException(
                    "Solo puedes iniciar el mantenimiento de un vehículo que esté Disponible.");
            }

            mantenimiento.Estado = "En proceso";
            mantenimiento.FechaInicio = DateTime.Now;
            mantenimiento.FechaFin = null;

            vehiculo.EstadoVehiculoId = estadoMantenimiento.Id;

            await _repository.GuardarCambiosAsync();
        }

        public async Task FinalizarAsync(int id)
        {
            var mantenimiento = await _context.Set<Mantenimiento>()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mantenimiento == null)
            {
                throw new KeyNotFoundException(
                    "El mantenimiento seleccionado no existe.");
            }

            if (mantenimiento.Estado != "En proceso")
            {
                throw new InvalidOperationException(
                    "Solo puedes finalizar mantenimientos que estén en proceso.");
            }

            var vehiculo = await _context.Set<Vehiculo>()
                .FirstOrDefaultAsync(v => v.Id == mantenimiento.VehiculoId);

            if (vehiculo == null)
            {
                throw new InvalidOperationException(
                    "El vehículo asociado al mantenimiento no existe.");
            }

            if (await _repository.ExisteMantenimientoActivoAsync(
                vehiculo.Id,
                mantenimiento.Id))
            {
                throw new InvalidOperationException(
                    "El vehículo tiene otro mantenimiento en proceso.");
            }

            var estados = await _context.Set<EstadoVehiculo>()
                .AsNoTracking()
                .ToListAsync();

            var disponible = estados.FirstOrDefault(e =>
                NormalizarEstado(e.Nombre) == "DISPONIBLE");

            var mantenimientoEstado = estados.FirstOrDefault(e =>
                NormalizarEstado(e.Nombre) == "EN MANTENIMIENTO" ||
                NormalizarEstado(e.Nombre) == "MANTENIMIENTO");

            if (disponible == null)
            {
                throw new InvalidOperationException(
                    "No existe el estado Disponible en el catálogo de vehículos.");
            }

            if (mantenimientoEstado == null)
            {
                throw new InvalidOperationException(
                    "No existe el estado En mantenimiento o Mantenimiento en el catálogo de vehículos.");
            }

            mantenimiento.Estado = "Finalizado";
            mantenimiento.FechaFin = DateTime.Now;

            if (vehiculo.EstadoVehiculoId == mantenimientoEstado.Id &&
                vehiculo.Activo)
            {
                vehiculo.EstadoVehiculoId = disponible.Id;
            }

            await _repository.GuardarCambiosAsync();
        }

        private async Task ValidarDatosAsync(
            MantenimientoDto dto,
            int excluirId = 0)
        {
            if (string.IsNullOrWhiteSpace(dto.Tipo))
            {
                throw new InvalidOperationException(
                    "El tipo de mantenimiento es obligatorio.");
            }

            if (dto.Tipo.Trim().Length > 100)
            {
                throw new InvalidOperationException(
                    "El tipo de mantenimiento no puede superar los 100 caracteres.");
            }

            if (dto.FechaInicio == default)
            {
                throw new InvalidOperationException(
                    "Selecciona la fecha de inicio.");
            }

            if (dto.CostoTotal < 0 || dto.CostoTotal > 100000000m)
            {
                throw new InvalidOperationException(
                    "El costo total debe estar entre 0 y 100000000.");
            }

            if (dto.Observaciones?.Trim().Length > 500)
            {
                throw new InvalidOperationException(
                    "Las observaciones no pueden superar los 500 caracteres.");
            }

            var vehiculo = await _repository.ObtenerVehiculoAsync(
                dto.VehiculoId);

            if (vehiculo == null)
            {
                throw new InvalidOperationException(
                    "El vehículo seleccionado no existe.");
            }

            if (!vehiculo.Activo)
            {
                throw new InvalidOperationException(
                    "No puedes asignar un mantenimiento a un vehículo inactivo.");
            }

            var empleado = await _repository.ObtenerEmpleadoAsync(
                dto.EmpleadoId);

            if (empleado == null)
            {
                throw new InvalidOperationException(
                    "El empleado seleccionado no existe.");
            }

            if (!empleado.Activo)
            {
                throw new InvalidOperationException(
                    "No puedes asignar un empleado inactivo.");
            }

            if (await _repository.ExisteMantenimientoActivoAsync(
                dto.VehiculoId,
                excluirId))
            {
                throw new InvalidOperationException(
                    "El vehículo tiene otro mantenimiento en proceso.");
            }
        }

        private static string NormalizarEstado(string valor)
        {
            return valor
                .Trim()
                .TrimEnd('.')
                .Trim()
                .ToUpperInvariant();
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        private static MantenimientoDto Mapear(Mantenimiento mantenimiento)
        {
            return new MantenimientoDto
            {
                Id = mantenimiento.Id,
                VehiculoId = mantenimiento.VehiculoId,
                EmpleadoId = mantenimiento.EmpleadoId,
                Tipo = mantenimiento.Tipo,
                FechaInicio = mantenimiento.FechaInicio,
                FechaFin = mantenimiento.FechaFin,
                CostoTotal = mantenimiento.CostoTotal,
                Estado = mantenimiento.Estado,
                Observaciones = mantenimiento.Observaciones,
                NombreVehiculo = mantenimiento.Vehiculo?.Modelo?.Nombre
                    ?? string.Empty,
                PlacaVehiculo = mantenimiento.Vehiculo?.Placa
                    ?? string.Empty,
                NombreEmpleado = mantenimiento.Empleado == null
                    ? string.Empty
                    : $"{mantenimiento.Empleado.Nombres} " +
                      $"{mantenimiento.Empleado.Apellidos}"
            };
        }
    }
}