
using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;

namespace AutoRentExpress.Services
{
    public class ContratoService : IContratoService
    {
        private readonly IContratoRepository _repository;

        public ContratoService(IContratoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ContratoDto>> ObtenerTodosAsync()
        {
            var contratos = await _repository.ObtenerTodosAsync();
            return contratos.Select(Mapear).ToList();
        }

        public async Task<ContratoDto?> ObtenerPorIdAsync(int id)
        {
            var contrato = await _repository.ObtenerPorIdAsync(id);
            return contrato == null ? null : Mapear(contrato);
        }

        public async Task<ContratoCatalogosDto> ObtenerCatalogosAsync()
        {
            var reservas = await _repository.ObtenerReservasSinContratoAsync();
            var empleados = await _repository.ObtenerEmpleadosAsync();

            return new ContratoCatalogosDto
            {
                Reservas = reservas.Select(r => new ContratoOpcionDto
                {
                    Id = r.Id,
                    Nombre = $"Reserva #{r.Id} - " +
                             $"{r.Cliente?.Nombres} {r.Cliente?.Apellidos} - " +
                             $"{r.Vehiculo?.Placa} - " +
                             $"{r.FechaInicio:dd/MM/yyyy HH:mm}"
                }).ToList(),

                Empleados = empleados.Select(e => new ContratoOpcionDto
                {
                    Id = e.Id,
                    Nombre = $"{e.Nombres} {e.Apellidos} - {e.Cargo}"
                }).ToList()
            };
        }

        public async Task CrearAsync(ContratoDto dto)
        {
            var reserva = await _repository.ObtenerReservaAsync(dto.ReservaId);

            if (reserva == null)
            {
                throw new InvalidOperationException(
                    "La reserva seleccionada no existe.");
            }

            if (reserva.Estado != "Pendiente")
            {
                throw new InvalidOperationException(
                    "Solo se pueden generar contratos para reservas pendientes.");
            }

            if (reserva.FechaInicio <= DateTime.Now)
            {
                throw new InvalidOperationException(
                    "No se puede generar un contrato para una reserva cuya fecha de retiro ya pasó.");
            }

            if (reserva.FechaFin <= reserva.FechaInicio)
            {
                throw new InvalidOperationException(
                    "Las fechas de la reserva no son válidas.");
            }

            if (await _repository.ExisteContratoReservaAsync(dto.ReservaId))
            {
                throw new InvalidOperationException(
                    "Esta reserva ya tiene un contrato registrado.");
            }

            var empleados = await _repository.ObtenerEmpleadosAsync();

            if (!empleados.Any(e => e.Id == dto.EmpleadoId))
            {
                throw new InvalidOperationException(
                    "Selecciona un empleado activo.");
            }

            var numero = GenerarNumeroContrato();

            while (await _repository.ExisteNumeroContratoAsync(numero))
            {
                numero = GenerarNumeroContrato();
            }

            var contrato = new Contrato
            {
                ReservaId = reserva.Id,
                EmpleadoId = dto.EmpleadoId,
                NumeroContrato = numero,
                FechaFirma = DateTime.Now,
                FechaEntrega = reserva.FechaInicio,
                FechaDevolucionPrevista = reserva.FechaFin,
                FechaDevolucionReal = null,
                TotalContrato = reserva.TotalEstimado,
                Estado = "Pendiente de entrega"
            };

            await _repository.AgregarAsync(contrato);
        }

        public async Task ConfirmarEntregaAsync(int id)
        {
            var contrato = await _repository.ObtenerPorIdAsync(id);

            if (contrato == null)
            {
                throw new KeyNotFoundException("El contrato no existe.");
            }

            if (contrato.Estado != "Pendiente de entrega")
            {
                throw new InvalidOperationException(
                    "Solo puedes entregar vehículos de contratos pendientes de entrega.");
            }

            var reserva = contrato.Reserva;
            var vehiculo = reserva?.Vehiculo;

            if (reserva == null || vehiculo == null)
            {
                throw new InvalidOperationException(
                    "El contrato no tiene una reserva y un vehículo válidos.");
            }

            if (reserva.Estado != "Pendiente" &&
                reserva.Estado != "Confirmada")
            {
                throw new InvalidOperationException(
                    "La reserva no está disponible para confirmar la entrega.");
            }

            if (!vehiculo.Activo)
            {
                throw new InvalidOperationException(
                    "El vehículo se encuentra inactivo.");
            }

            if (await _repository.ExisteOtroContratoEnCursoAsync(
                vehiculo.Id, contrato.Id))
            {
                throw new InvalidOperationException(
                    "Este vehículo ya está entregado en otro contrato.");
            }

            if (!await _repository.ExisteInspeccionAsync(
                contrato.Id, "Entrega"))
            {
                throw new InvalidOperationException(
                    "Debes registrar primero la inspección de entrega.");
            }

            var estadoActual = await _repository.ObtenerEstadoVehiculoAsync(
                "Alquilado");

            var disponible = await _repository.ObtenerEstadoVehiculoAsync(
                "Disponible");

            var reservado = await _repository.ObtenerEstadoVehiculoAsync(
                "Reservado");

            if (estadoActual == null)
            {
                throw new InvalidOperationException(
                    "No existe el estado Alquilado en el catálogo de vehículos.");
            }

            if (vehiculo.EstadoVehiculoId != disponible?.Id &&
                vehiculo.EstadoVehiculoId != reservado?.Id)
            {
                throw new InvalidOperationException(
                    "El vehículo no está en un estado que permita su entrega.");
            }

            var ahora = DateTime.Now;

            contrato.Estado = "En curso";
            contrato.FechaEntrega = ahora;
            contrato.FechaDevolucionReal = null;

            reserva.Estado = "En curso";
            vehiculo.EstadoVehiculoId = estadoActual.Id;

            await _repository.GuardarCambiosAsync();
        }

        public async Task ConfirmarDevolucionAsync(int id)
        {
            var contrato = await _repository.ObtenerPorIdAsync(id);

            if (contrato == null)
            {
                throw new KeyNotFoundException("El contrato no existe.");
            }

            if (contrato.Estado != "En curso")
            {
                throw new InvalidOperationException(
                    "Solo puedes devolver vehículos de contratos en curso.");
            }

            var reserva = contrato.Reserva;
            var vehiculo = reserva?.Vehiculo;

            if (reserva == null || vehiculo == null)
            {
                throw new InvalidOperationException(
                    "El contrato no tiene una reserva y un vehículo válidos.");
            }

            if (!await _repository.ExisteInspeccionAsync(
                contrato.Id, "Devolución"))
            {
                throw new InvalidOperationException(
                    "Debes registrar primero la inspección de devolución.");
            }

            if (reserva.Estado != "En curso")
            {
                throw new InvalidOperationException(
                    "La reserva no figura como alquiler en curso.");
            }

            var disponible = await _repository.ObtenerEstadoVehiculoAsync(
                "Disponible");

            if (disponible == null)
            {
                throw new InvalidOperationException(
                    "No existe el estado Disponible en el catálogo de vehículos.");
            }

            var alquilado = await _repository.ObtenerEstadoVehiculoAsync(
                "Alquilado");

            var ahora = DateTime.Now;

            contrato.FechaDevolucionReal = ahora;
            contrato.Estado = "Finalizado";

            reserva.Estado = "Finalizada";

            if (alquilado != null &&
                vehiculo.EstadoVehiculoId == alquilado.Id)
            {
                vehiculo.EstadoVehiculoId = disponible.Id;
            }

            await _repository.GuardarCambiosAsync();
        }

        private static string GenerarNumeroContrato()
        {
            return $"ARE-{DateTime.Now:yyyyMMdd}-" +
                   $"{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }

        private static ContratoDto Mapear(Contrato contrato)
        {
            return new ContratoDto
            {
                Id = contrato.Id,
                ReservaId = contrato.ReservaId,
                EmpleadoId = contrato.EmpleadoId,
                NumeroContrato = contrato.NumeroContrato,
                FechaFirma = contrato.FechaFirma,
                FechaEntrega = contrato.FechaEntrega,
                FechaDevolucionPrevista = contrato.FechaDevolucionPrevista,
                FechaDevolucionReal = contrato.FechaDevolucionReal,
                TotalContrato = contrato.TotalContrato,
                Estado = contrato.Estado,
                NombreCliente = contrato.Reserva?.Cliente == null
                    ? string.Empty
                    : $"{contrato.Reserva.Cliente.Nombres} " +
                      $"{contrato.Reserva.Cliente.Apellidos}",
                Vehiculo = contrato.Reserva?.Vehiculo?.Modelo?.Nombre
                    ?? string.Empty,
                PlacaVehiculo = contrato.Reserva?.Vehiculo?.Placa
                    ?? string.Empty
            };
        }
    }
}
