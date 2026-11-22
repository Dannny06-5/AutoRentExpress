
using AutoRentExpress.Data;
using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _repository;
        private readonly IContratoRepository _contratoRepository;
        private readonly AppDbContext _context;

        public ReservaService(
            IReservaRepository repository,
            IContratoRepository contratoRepository,
            AppDbContext context)
        {
            _repository = repository;
            _contratoRepository = contratoRepository;
            _context = context;
        }

        public async Task<List<ReservaDto>> ObtenerTodosAsync()
        {
            var reservas = await _repository.ObtenerTodosAsync();
            var contratos = await _contratoRepository.ObtenerTodosAsync();

            var idsConContrato = contratos
                .Select(c => c.ReservaId)
                .ToHashSet();

            var detalles = await _context.Set<ReservaDetalle>()
                .AsNoTracking()
                .Where(d => d.SeguroId != null)
                .Include(d => d.Seguro)
                .ToListAsync();

            return reservas.Select(reserva =>
            {
                var dto = Mapear(reserva);

                dto.TieneContrato = idsConContrato.Contains(reserva.Id);

                var detalle = detalles.FirstOrDefault(
                    d => d.ReservaId == reserva.Id);

                if (detalle != null)
                {
                    dto.SeguroId = detalle.SeguroId;
                    dto.NombreSeguro = detalle.Seguro?.Nombre
                        ?? detalle.Concepto;
                    dto.CostoSeguro = detalles
                        .Where(d => d.ReservaId == reserva.Id)
                        .Sum(d => d.Subtotal);
                }

                return dto;
            }).ToList();
        }

        public async Task<ReservaDto?> ObtenerPorIdAsync(int id)
        {
            var reserva = await _repository.ObtenerPorIdAsync(id);

            if (reserva == null)
            {
                return null;
            }

            var dto = Mapear(reserva);

            dto.TieneContrato =
                await _contratoRepository.ExisteContratoReservaAsync(id);

            var detalles = await _context.Set<ReservaDetalle>()
                .AsNoTracking()
                .Where(d => d.ReservaId == id && d.SeguroId != null)
                .Include(d => d.Seguro)
                .ToListAsync();

            var detalle = detalles.FirstOrDefault();

            if (detalle != null)
            {
                dto.SeguroId = detalle.SeguroId;
                dto.NombreSeguro = detalle.Seguro?.Nombre
                    ?? detalle.Concepto;
                dto.CostoSeguro = detalles.Sum(d => d.Subtotal);
            }

            return dto;
        }

        public async Task<ReservaCatalogosDto> ObtenerCatalogosAsync()
        {
            var clientes = await _repository.ObtenerClientesAsync();
            var vehiculos = await _repository.ObtenerVehiculosAsync();
            var sucursales = await _repository.ObtenerSucursalesAsync();

            var seguros = await _context.Set<Seguro>()
                .AsNoTracking()
                .Where(s => s.Activo)
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            return new ReservaCatalogosDto
            {
                Clientes = clientes.Select(c => new ReservaOpcionDto
                {
                    Id = c.Id,
                    Nombre = $"{c.Nombres} {c.Apellidos}"
                }).ToList(),

                Vehiculos = vehiculos.Select(v => new ReservaOpcionDto
                {
                    Id = v.Id,
                    Nombre = $"{v.Placa} - {v.Modelo?.Nombre}",
                    TarifaDiaria = v.TarifaDiaria
                }).ToList(),

                Sucursales = sucursales.Select(s => new ReservaOpcionDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre
                }).ToList(),

                Seguros = seguros.Select(s => new ReservaOpcionDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    TarifaDiaria = s.CostoDiario
                }).ToList()
            };
        }

        public async Task<List<ReservaOpcionDto>> ObtenerDisponiblesAsync(
            DateTime fechaInicio,
            DateTime fechaFin,
            int sucursalRetiroId,
            int reservaId = 0)
        {
            if (fechaInicio == default ||
                fechaFin == default ||
                fechaInicio >= fechaFin ||
                sucursalRetiroId <= 0)
            {
                return new List<ReservaOpcionDto>();
            }

            var vehiculos = await _repository.ObtenerVehiculosAsync();
            var reservas = await _repository.ObtenerTodosAsync();

            var disponibles = new List<ReservaOpcionDto>();

            foreach (var vehiculo in vehiculos)
            {
                if (!vehiculo.Activo ||
                    vehiculo.SucursalId != sucursalRetiroId ||
                    vehiculo.TarifaDiaria <= 0)
                {
                    continue;
                }

                var estado = await _repository.ObtenerEstadoVehiculoAsync(
                    vehiculo.EstadoVehiculoId);

                if (estado == null)
                {
                    continue;
                }

                var nombreEstado = estado.Nombre
                    .Trim()
                    .TrimEnd('.')
                    .Trim()
                    .ToUpperInvariant();

                if (nombreEstado != "DISPONIBLE" &&
                    nombreEstado != "RESERVADO")
                {
                    continue;
                }

                var ocupado = reservas.Any(r =>
                    r.VehiculoId == vehiculo.Id &&
                    r.Id != reservaId &&
                    r.Estado != "Cancelada" &&
                    r.FechaInicio < fechaFin &&
                    fechaInicio < r.FechaFin);

                if (ocupado)
                {
                    continue;
                }

                disponibles.Add(new ReservaOpcionDto
                {
                    Id = vehiculo.Id,
                    Nombre = $"{vehiculo.Placa} - {vehiculo.Modelo?.Nombre}",
                    TarifaDiaria = vehiculo.TarifaDiaria
                });
            }

            return disponibles.OrderBy(v => v.Nombre).ToList();
        }

        public async Task CrearAsync(ReservaDto dto)
        {
            var tarifa = await ValidarAsync(dto);

            var dias = CalcularDias(dto.FechaInicio, dto.FechaFin);

            var seguro = await ObtenerSeguroValidoAsync(dto.SeguroId);

            var costoSeguro = seguro == null
                ? 0m
                : dias * seguro.CostoDiario;

            var reserva = new Reserva
            {
                ClienteId = dto.ClienteId,
                VehiculoId = dto.VehiculoId,
                SucursalRetiroId = dto.SucursalRetiroId,
                SucursalDevolucionId = dto.SucursalDevolucionId,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                FechaRegistro = DateTime.Now,
                Estado = "Pendiente",
                TotalEstimado = CalcularTotal(
                    dto.FechaInicio,
                    dto.FechaFin,
                    tarifa) + costoSeguro
            };

            await using var transaccion =
                await _context.Database.BeginTransactionAsync();

            await _repository.AgregarAsync(reserva);

            if (seguro != null)
            {
                var detalle = new ReservaDetalle
                {
                    ReservaId = reserva.Id,
                    SeguroId = seguro.Id,
                    Concepto = $"Seguro: {seguro.Nombre}",
                    Cantidad = dias,
                    PrecioUnitario = seguro.CostoDiario,
                    Subtotal = costoSeguro
                };

                await _context.Set<ReservaDetalle>().AddAsync(detalle);
                await _context.SaveChangesAsync();
            }

            await transaccion.CommitAsync();
        }

        public async Task ActualizarAsync(ReservaDto dto)
        {
            var reserva = await _repository.ObtenerPorIdAsync(dto.Id);

            if (reserva == null)
            {
                throw new KeyNotFoundException(
                    "La reserva no existe.");
            }

            if (reserva.Estado != "Pendiente" &&
                reserva.Estado != "Confirmada")
            {
                throw new InvalidOperationException(
                    "Solo puedes editar reservas pendientes o confirmadas.");
            }

            if (await _contratoRepository.ExisteContratoReservaAsync(
                reserva.Id))
            {
                throw new InvalidOperationException(
                    "No puedes editar una reserva que ya tiene un contrato registrado.");
            }

            var tarifa = await ValidarAsync(dto, dto.Id);

            var dias = CalcularDias(dto.FechaInicio, dto.FechaFin);

            var detallesSeguro = await _context.Set<ReservaDetalle>()
                .Where(d =>
                    d.ReservaId == reserva.Id &&
                    d.SeguroId != null)
                .ToListAsync();

            foreach (var detalle in detallesSeguro)
            {
                detalle.Cantidad = dias;
                detalle.Subtotal = dias * detalle.PrecioUnitario;
            }

            var costoSeguro = detallesSeguro.Sum(d => d.Subtotal);

            reserva.ClienteId = dto.ClienteId;
            reserva.VehiculoId = dto.VehiculoId;
            reserva.SucursalRetiroId = dto.SucursalRetiroId;
            reserva.SucursalDevolucionId = dto.SucursalDevolucionId;
            reserva.FechaInicio = dto.FechaInicio;
            reserva.FechaFin = dto.FechaFin;
            reserva.TotalEstimado = CalcularTotal(
                dto.FechaInicio,
                dto.FechaFin,
                tarifa) + costoSeguro;

            await _repository.GuardarCambiosAsync();
        }

        public async Task CancelarAsync(int id)
        {
            var reserva = await _repository.ObtenerPorIdAsync(id);

            if (reserva == null)
            {
                throw new KeyNotFoundException(
                    "La reserva no existe.");
            }

            if (reserva.Estado == "Cancelada")
            {
                throw new InvalidOperationException(
                    "Esta reserva ya está cancelada.");
            }

            if (reserva.Estado != "Pendiente" &&
                reserva.Estado != "Confirmada")
            {
                throw new InvalidOperationException(
                    "No puedes cancelar una reserva cuyo alquiler ya comenzó, terminó o no permite cancelación.");
            }

            if (await _contratoRepository.ExisteContratoReservaAsync(
                reserva.Id))
            {
                throw new InvalidOperationException(
                    "No puedes cancelar una reserva que ya tiene un contrato registrado.");
            }

            reserva.Estado = "Cancelada";

            await _repository.GuardarCambiosAsync();
        }

        private async Task<Seguro?> ObtenerSeguroValidoAsync(int? seguroId)
        {
            if (!seguroId.HasValue || seguroId.Value == 0)
            {
                return null;
            }

            var seguro = await _context.Set<Seguro>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == seguroId.Value);

            if (seguro == null)
            {
                throw new InvalidOperationException(
                    "El seguro seleccionado no existe.");
            }

            if (!seguro.Activo)
            {
                throw new InvalidOperationException(
                    "El seguro seleccionado se encuentra inactivo.");
            }

            if (seguro.CostoDiario < 0 ||
                seguro.CostoDiario > 1000000m)
            {
                throw new InvalidOperationException(
                    "El seguro seleccionado tiene un costo diario inválido.");
            }

            return seguro;
        }

        private async Task<decimal> ValidarAsync(
            ReservaDto dto,
            int excluirReservaId = 0)
        {
            if (dto.FechaInicio == default ||
                dto.FechaFin == default)
            {
                throw new InvalidOperationException(
                    "Debes indicar ambas fechas de la reserva.");
            }

            if (dto.FechaInicio >= dto.FechaFin)
            {
                throw new InvalidOperationException(
                    "La fecha de devolución debe ser posterior a la fecha de retiro.");
            }

            var ahora = DateTime.Now;

            if (excluirReservaId == 0)
            {
                if (dto.FechaInicio < ahora)
                {
                    throw new InvalidOperationException(
                        "No puedes registrar una reserva con fecha de inicio pasada.");
                }
            }
            else
            {
                var reservaOriginal =
                    await _repository.ObtenerPorIdAsync(excluirReservaId);

                if (reservaOriginal == null)
                {
                    throw new InvalidOperationException(
                        "La reserva que intentas editar no existe.");
                }

                if (dto.FechaInicio != reservaOriginal.FechaInicio)
                {
                    if (reservaOriginal.FechaInicio < ahora)
                    {
                        throw new InvalidOperationException(
                            "No puedes modificar la fecha de retiro de una reserva que ya comenzó.");
                    }

                    if (dto.FechaInicio < ahora)
                    {
                        throw new InvalidOperationException(
                            "No puedes cambiar la fecha de retiro a una fecha u hora pasada.");
                    }
                }
            }

            if (dto.FechaFin <= ahora)
            {
                throw new InvalidOperationException(
                    "La fecha de devolución debe ser posterior a la fecha y hora actual.");
            }

            var cliente =
                await _repository.ObtenerClienteAsync(dto.ClienteId);

            if (cliente == null)
            {
                throw new InvalidOperationException(
                    "El cliente seleccionado no existe.");
            }

            if (!cliente.Activo)
            {
                throw new InvalidOperationException(
                    "No puedes registrar reservas para un cliente inactivo.");
            }

            var vehiculo =
                await _repository.ObtenerVehiculoAsync(dto.VehiculoId);

            if (vehiculo == null)
            {
                throw new InvalidOperationException(
                    "El vehículo seleccionado no existe.");
            }

            if (!vehiculo.Activo)
            {
                throw new InvalidOperationException(
                    "No puedes reservar un vehículo desactivado.");
            }

            var estadoVehiculo =
                await _repository.ObtenerEstadoVehiculoAsync(
                    vehiculo.EstadoVehiculoId);

            if (estadoVehiculo == null)
            {
                throw new InvalidOperationException(
                    "El vehículo no tiene un estado válido.");
            }

            var nombreEstado = estadoVehiculo.Nombre
                .Trim()
                .TrimEnd('.')
                .Trim()
                .ToUpperInvariant();

            if (nombreEstado == "MANTENIMIENTO" ||
                nombreEstado == "EN MANTENIMIENTO" ||
                nombreEstado == "ALQUILADO" ||
                nombreEstado == "FUERA DE SERVICIO")
            {
                throw new InvalidOperationException(
                    $"No puedes reservar el vehículo porque su estado actual es: {estadoVehiculo.Nombre}.");
            }

            if (nombreEstado != "DISPONIBLE" &&
                nombreEstado != "RESERVADO")
            {
                throw new InvalidOperationException(
                    $"El estado {estadoVehiculo.Nombre} no permite realizar reservas.");
            }

            if (vehiculo.TarifaDiaria <= 0)
            {
                throw new InvalidOperationException(
                    "El vehículo no tiene una tarifa diaria válida.");
            }

            var sucursalRetiro =
                await _repository.ObtenerSucursalAsync(
                    dto.SucursalRetiroId);

            if (sucursalRetiro == null || !sucursalRetiro.Activa)
            {
                throw new InvalidOperationException(
                    "Selecciona una sucursal de retiro activa.");
            }

            var sucursalDevolucion =
                await _repository.ObtenerSucursalAsync(
                    dto.SucursalDevolucionId);

            if (sucursalDevolucion == null ||
                !sucursalDevolucion.Activa)
            {
                throw new InvalidOperationException(
                    "Selecciona una sucursal de devolución activa.");
            }

            if (vehiculo.SucursalId != dto.SucursalRetiroId)
            {
                throw new InvalidOperationException(
                    "El vehículo no pertenece a la sucursal de retiro seleccionada.");
            }

            var tieneConflicto =
                await _repository.ExisteConflictoAsync(
                    dto.VehiculoId,
                    dto.FechaInicio,
                    dto.FechaFin,
                    excluirReservaId);

            if (tieneConflicto)
            {
                throw new InvalidOperationException(
                    "El vehículo ya tiene una reserva para las fechas seleccionadas.");
            }

            return vehiculo.TarifaDiaria;
        }

        private static int CalcularDias(
            DateTime inicio,
            DateTime fin)
        {
            var duracion = fin - inicio;

            return Math.Max(
                1,
                (int)Math.Ceiling(duracion.TotalDays));
        }

        private static decimal CalcularTotal(
            DateTime inicio,
            DateTime fin,
            decimal tarifaDiaria)
        {
            return CalcularDias(inicio, fin) * tarifaDiaria;
        }

        private static ReservaDto Mapear(Reserva reserva)
        {
            return new ReservaDto
            {
                Id = reserva.Id,
                ClienteId = reserva.ClienteId,
                VehiculoId = reserva.VehiculoId,
                SucursalRetiroId = reserva.SucursalRetiroId,
                SucursalDevolucionId = reserva.SucursalDevolucionId,
                FechaInicio = reserva.FechaInicio,
                FechaFin = reserva.FechaFin,
                FechaRegistro = reserva.FechaRegistro,
                Estado = reserva.Estado,
                TotalEstimado = reserva.TotalEstimado,

                NombreCliente = reserva.Cliente == null
                    ? string.Empty
                    : $"{reserva.Cliente.Nombres} {reserva.Cliente.Apellidos}",

                NombreVehiculo = reserva.Vehiculo?.Modelo?.Nombre
                    ?? string.Empty,

                PlacaVehiculo = reserva.Vehiculo?.Placa
                    ?? string.Empty,

                NombreSucursalRetiro =
                    reserva.SucursalRetiro?.Nombre ?? string.Empty,

                NombreSucursalDevolucion =
                    reserva.SucursalDevolucion?.Nombre ?? string.Empty
            };
        }
    }
}
