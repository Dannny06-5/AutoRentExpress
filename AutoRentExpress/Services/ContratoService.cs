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
                    "No se puede generar un contrato nuevo para una reserva que ya comenzó.");
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
                    "El empleado seleccionado no existe.");
            }

            var numeroContrato = GenerarNumeroContrato();

            while (await _repository.ExisteNumeroContratoAsync(numeroContrato))
            {
                numeroContrato = GenerarNumeroContrato();
            }

            var contrato = new Contrato
            {
                ReservaId = reserva.Id,
                EmpleadoId = dto.EmpleadoId,
                NumeroContrato = numeroContrato,
                FechaFirma = DateTime.Now,
                FechaEntrega = reserva.FechaInicio,
                FechaDevolucionPrevista = reserva.FechaFin,
                FechaDevolucionReal = null,
                TotalContrato = reserva.TotalEstimado,
                Estado = "Pendiente de entrega"
            };

            await _repository.AgregarAsync(contrato);
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