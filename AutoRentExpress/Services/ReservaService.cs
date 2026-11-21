using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;

namespace AutoRentExpress.Services
{
    public class ReservaService : IReservaService
    {
        private readonly IReservaRepository _repository;

        public ReservaService(IReservaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ReservaDto>> ObtenerTodosAsync()
        {
            var reservas = await _repository.ObtenerTodosAsync();
            return reservas.Select(Mapear).ToList();
        }

        public async Task<ReservaDto?> ObtenerPorIdAsync(int id)
        {
            var reserva = await _repository.ObtenerPorIdAsync(id);
            return reserva == null ? null : Mapear(reserva);
        }

        public async Task<ReservaCatalogosDto> ObtenerCatalogosAsync()
        {
            var clientes = await _repository.ObtenerClientesAsync();
            var vehiculos = await _repository.ObtenerVehiculosAsync();
            var sucursales = await _repository.ObtenerSucursalesAsync();

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
                    continue;

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
            var tarifa = await ValidarAsync(dto);

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
                    tarifa)
            };

            await _repository.AgregarAsync(reserva);
        }

        public async Task ActualizarAsync(ReservaDto dto)
        {
            var reserva = await _repository.ObtenerPorIdAsync(dto.Id);

            if (reserva == null)
            {
                throw new KeyNotFoundException(
                    "La reserva no existe.");
            }

            if (reserva.Estado == "Cancelada")
            {
                throw new InvalidOperationException(
                    "No es posible editar una reserva cancelada.");
            }

            var tarifa = await ValidarAsync(dto, dto.Id);

            reserva.ClienteId = dto.ClienteId;
            reserva.VehiculoId = dto.VehiculoId;
            reserva.SucursalRetiroId = dto.SucursalRetiroId;
            reserva.SucursalDevolucionId = dto.SucursalDevolucionId;
            reserva.FechaInicio = dto.FechaInicio;
            reserva.FechaFin = dto.FechaFin;
            reserva.TotalEstimado = CalcularTotal(
                dto.FechaInicio,
                dto.FechaFin,
                tarifa);

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

            if (reserva.Estado == "En curso" ||
                reserva.Estado == "Finalizada")
            {
                throw new InvalidOperationException(
                    "No puedes cancelar una reserva cuyo alquiler ya comenzó o terminó.");
            }

            if (reserva.Estado != "Pendiente" &&
                reserva.Estado != "Confirmada")
            {
                throw new InvalidOperationException(
                    "El estado actual de la reserva no permite cancelarla.");
            }

            reserva.Estado = "Cancelada";

            await _repository.GuardarCambiosAsync();
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

        private static decimal CalcularTotal(
            DateTime inicio,
            DateTime fin,
            decimal tarifaDiaria)
        {
            var duracion = fin - inicio;

            var dias = Math.Max(
                1,
                (int)Math.Ceiling(duracion.TotalDays));

            return dias * tarifaDiaria;
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