
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

        private const string ConceptoAlquiler =
            "Alquiler de vehículo - tarifa aplicada";

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

            var seguros = await _context.ReservaDetalles
                .AsNoTracking()
                .Where(d => d.SeguroId != null)
                .Include(d => d.Seguro)
                .ToListAsync();

            return reservas.Select(reserva =>
            {
                var dto = Mapear(reserva);

                dto.TieneContrato =
                    idsConContrato.Contains(reserva.Id);

                var detallesSeguro = seguros
                    .Where(d => d.ReservaId == reserva.Id)
                    .ToList();

                if (detallesSeguro.Count > 0)
                {
                    var primero = detallesSeguro[0];

                    dto.SeguroId = primero.SeguroId;
                    dto.NombreSeguro = primero.Seguro?.Nombre
                        ?? primero.Concepto;
                    dto.CostoSeguro = detallesSeguro
                        .Sum(d => d.Subtotal);
                }

                return dto;
            }).ToList();
        }

        public async Task<ReservaDto?> ObtenerPorIdAsync(int id)
        {
            var reserva = await _repository.ObtenerPorIdAsync(id);

            if (reserva == null)
                return null;

            var dto = Mapear(reserva);

            dto.TieneContrato =
                await _contratoRepository.ExisteContratoReservaAsync(id);

            var seguros = await _context.ReservaDetalles
                .AsNoTracking()
                .Where(d =>
                    d.ReservaId == id &&
                    d.SeguroId != null)
                .Include(d => d.Seguro)
                .ToListAsync();

            if (seguros.Count > 0)
            {
                dto.SeguroId = seguros[0].SeguroId;
                dto.NombreSeguro = seguros[0].Seguro?.Nombre
                    ?? seguros[0].Concepto;
                dto.CostoSeguro = seguros.Sum(d => d.Subtotal);
            }

            return dto;
        }

        public async Task<ReservaCatalogosDto> ObtenerCatalogosAsync()
        {
            var clientes = await _repository.ObtenerClientesAsync();
            var vehiculos = await _repository.ObtenerVehiculosAsync();
            var sucursales = await _repository.ObtenerSucursalesAsync();

            var seguros = await _context.Seguros
                .AsNoTracking()
                .Where(s => s.Activo)
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            var tarifas = await _context.Tarifas
                .AsNoTracking()
                .Where(t => t.Activa && t.PrecioDiario > 0)
                .OrderBy(t => t.Nombre)
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
                    TarifaDiaria = v.TarifaDiaria,
                    CategoriaVehiculoId = v.CategoriaVehiculoId
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
                }).ToList(),

                Tarifas = tarifas.Select(t => new ReservaTarifaOpcionDto
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    CategoriaVehiculoId = t.CategoriaVehiculoId,
                    PrecioDiario = t.PrecioDiario,
                    VigenteDesde = t.VigenteDesde,
                    VigenteHasta = t.VigenteHasta
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

                var estado =
                    await _repository.ObtenerEstadoVehiculoAsync(
                        vehiculo.EstadoVehiculoId);

                if (estado == null)
                    continue;

                var nombreEstado = NormalizarEstado(estado.Nombre);

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
                    continue;

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
            var vehiculo = await ValidarAsync(dto);

            var precio = await ObtenerPrecioAplicableAsync(
                vehiculo,
                dto.FechaInicio,
                dto.FechaFin,
                dto.TarifaId);

            var dias = CalcularDias(dto.FechaInicio, dto.FechaFin);

            var seguro = await ObtenerSeguroValidoAsync(dto.SeguroId);

            var costoAlquiler = dias * precio;

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
                TotalEstimado = costoAlquiler + costoSeguro
            };

            await using var transaccion =
                await _context.Database.BeginTransactionAsync();

            await _repository.AgregarAsync(reserva);

            await _context.ReservaDetalles.AddAsync(
                new ReservaDetalle
                {
                    ReservaId = reserva.Id,
                    SeguroId = null,
                    Concepto = ConceptoAlquiler,
                    Cantidad = dias,
                    PrecioUnitario = precio,
                    Subtotal = costoAlquiler
                });

            if (seguro != null)
            {
                await _context.ReservaDetalles.AddAsync(
                    new ReservaDetalle
                    {
                        ReservaId = reserva.Id,
                        SeguroId = seguro.Id,
                        Concepto = $"Seguro: {seguro.Nombre}",
                        Cantidad = dias,
                        PrecioUnitario = seguro.CostoDiario,
                        Subtotal = costoSeguro
                    });
            }

            await _context.SaveChangesAsync();
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

            var vehiculo = await ValidarAsync(dto, dto.Id);

            var mismoVehiculo =
                reserva.VehiculoId == dto.VehiculoId;

            decimal precio;

            if (dto.TarifaId.HasValue && dto.TarifaId.Value > 0)
            {
                precio = await ObtenerPrecioAplicableAsync(
                    vehiculo,
                    dto.FechaInicio,
                    dto.FechaFin,
                    dto.TarifaId);
            }
            else if (mismoVehiculo)
            {
                precio = await ObtenerPrecioHistoricoAsync(reserva);
            }
            else
            {
                precio = vehiculo.TarifaDiaria;
            }

            var dias = CalcularDias(dto.FechaInicio, dto.FechaFin);

            var detalles = await _context.ReservaDetalles
                .Where(d => d.ReservaId == reserva.Id)
                .ToListAsync();

            var detallesSeguro = detalles
                .Where(d => d.SeguroId != null)
                .ToList();

            foreach (var detalle in detallesSeguro)
            {
                detalle.Cantidad = dias;
                detalle.Subtotal = dias * detalle.PrecioUnitario;
            }

            var detalleAlquiler = detalles.FirstOrDefault(d =>
                d.SeguroId == null &&
                d.Concepto == ConceptoAlquiler);

            if (detalleAlquiler == null)
            {
                detalleAlquiler = new ReservaDetalle
                {
                    ReservaId = reserva.Id,
                    SeguroId = null,
                    Concepto = ConceptoAlquiler
                };

                await _context.ReservaDetalles.AddAsync(
                    detalleAlquiler);
            }

            detalleAlquiler.Cantidad = dias;
            detalleAlquiler.PrecioUnitario = precio;
            detalleAlquiler.Subtotal = dias * precio;

            var costoSeguro = detallesSeguro.Sum(d => d.Subtotal);

            reserva.ClienteId = dto.ClienteId;
            reserva.VehiculoId = dto.VehiculoId;
            reserva.SucursalRetiroId = dto.SucursalRetiroId;
            reserva.SucursalDevolucionId = dto.SucursalDevolucionId;
            reserva.FechaInicio = dto.FechaInicio;
            reserva.FechaFin = dto.FechaFin;
            reserva.TotalEstimado =
                detalleAlquiler.Subtotal + costoSeguro;

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

        private async Task<decimal> ObtenerPrecioAplicableAsync(
            Vehiculo vehiculo,
            DateTime inicio,
            DateTime fin,
            int? tarifaId)
        {
            if (!tarifaId.HasValue || tarifaId.Value == 0)
            {
                return vehiculo.TarifaDiaria;
            }

            var tarifa = await _context.Tarifas
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tarifaId.Value);

            if (tarifa == null)
            {
                throw new InvalidOperationException(
                    "La tarifa seleccionada no existe.");
            }

            if (!tarifa.Activa)
            {
                throw new InvalidOperationException(
                    "La tarifa seleccionada está inactiva.");
            }

            if (tarifa.CategoriaVehiculoId !=
                vehiculo.CategoriaVehiculoId)
            {
                throw new InvalidOperationException(
                    "La tarifa no corresponde a la categoría del vehículo.");
            }

            var primerDia = inicio.Date;

            var ultimoDia = fin.TimeOfDay == TimeSpan.Zero
                ? fin.Date.AddDays(-1)
                : fin.Date;

            if (tarifa.VigenteDesde.Date > primerDia ||
                (tarifa.VigenteHasta.HasValue &&
                 tarifa.VigenteHasta.Value.Date < ultimoDia))
            {
                throw new InvalidOperationException(
                    "La tarifa seleccionada no cubre todo el período de alquiler.");
            }

            if (tarifa.PrecioDiario <= 0 ||
                tarifa.PrecioDiario > 1000000m)
            {
                throw new InvalidOperationException(
                    "La tarifa tiene un precio diario inválido.");
            }

            return tarifa.PrecioDiario;
        }

        private async Task<decimal> ObtenerPrecioHistoricoAsync(
            Reserva reserva)
        {
            var detalle = await _context.ReservaDetalles
                .AsNoTracking()
                .FirstOrDefaultAsync(d =>
                    d.ReservaId == reserva.Id &&
                    d.SeguroId == null &&
                    d.Concepto == ConceptoAlquiler);

            if (detalle != null)
            {
                return detalle.PrecioUnitario;
            }

            var seguro = await _context.ReservaDetalles
                .AsNoTracking()
                .Where(d =>
                    d.ReservaId == reserva.Id &&
                    d.SeguroId != null)
                .SumAsync(d => (decimal?)d.Subtotal) ?? 0m;

            var dias = CalcularDias(
                reserva.FechaInicio,
                reserva.FechaFin);

            var alquiler = reserva.TotalEstimado - seguro;

            if (alquiler <= 0)
            {
                throw new InvalidOperationException(
                    "No se pudo recuperar el precio original de la reserva.");
            }

            return Math.Round(
                alquiler / dias,
                2,
                MidpointRounding.AwayFromZero);
        }

        private async Task<Seguro?> ObtenerSeguroValidoAsync(
            int? seguroId)
        {
            if (!seguroId.HasValue || seguroId.Value == 0)
            {
                return null;
            }

            var seguro = await _context.Seguros
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

        private async Task<Vehiculo> ValidarAsync(
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
                var original =
                    await _repository.ObtenerPorIdAsync(
                        excluirReservaId);

                if (original == null)
                {
                    throw new InvalidOperationException(
                        "La reserva que intentas editar no existe.");
                }

                if (dto.FechaInicio != original.FechaInicio)
                {
                    if (original.FechaInicio < ahora)
                    {
                        throw new InvalidOperationException(
                            "No puedes modificar la fecha de retiro de una reserva que ya comenzó.");
                    }

                    if (dto.FechaInicio < ahora)
                    {
                        throw new InvalidOperationException(
                            "No puedes cambiar la fecha de retiro a una fecha pasada.");
                    }
                }
            }

            if (dto.FechaFin <= ahora)
            {
                throw new InvalidOperationException(
                    "La fecha de devolución debe ser posterior a la fecha actual.");
            }

            var cliente =
                await _repository.ObtenerClienteAsync(
                    dto.ClienteId);

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
                await _repository.ObtenerVehiculoAsync(
                    dto.VehiculoId);

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

            var estado = NormalizarEstado(
                estadoVehiculo.Nombre);

            if (estado == "MANTENIMIENTO" ||
                estado == "EN MANTENIMIENTO" ||
                estado == "ALQUILADO" ||
                estado == "FUERA DE SERVICIO")
            {
                throw new InvalidOperationException(
                    $"No puedes reservar el vehículo porque su estado actual es: {estadoVehiculo.Nombre}.");
            }

            if (estado != "DISPONIBLE" &&
                estado != "RESERVADO")
            {
                throw new InvalidOperationException(
                    $"El estado {estadoVehiculo.Nombre} no permite reservas.");
            }

            if (vehiculo.TarifaDiaria <= 0)
            {
                throw new InvalidOperationException(
                    "El vehículo no tiene una tarifa diaria válida.");
            }

            var sucursalRetiro =
                await _repository.ObtenerSucursalAsync(
                    dto.SucursalRetiroId);

            if (sucursalRetiro == null ||
                !sucursalRetiro.Activa)
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

            var conflicto =
                await _repository.ExisteConflictoAsync(
                    dto.VehiculoId,
                    dto.FechaInicio,
                    dto.FechaFin,
                    excluirReservaId);

            if (conflicto)
            {
                throw new InvalidOperationException(
                    "El vehículo ya tiene una reserva para las fechas seleccionadas.");
            }

            return vehiculo;
        }

        private static string NormalizarEstado(string? estado)
        {
            return (estado ?? string.Empty)
                .Trim()
                .TrimEnd('.')
                .Trim()
                .ToUpperInvariant();
        }

        private static int CalcularDias(
            DateTime inicio,
            DateTime fin)
        {
            return Math.Max(
                1,
                (int)Math.Ceiling(
                    (fin - inicio).TotalDays));
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
