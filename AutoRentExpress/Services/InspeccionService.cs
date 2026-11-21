using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;

namespace AutoRentExpress.Services
{
    public class InspeccionService : IInspeccionService
    {
        private readonly IInspeccionRepository _repository;

        public InspeccionService(IInspeccionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<InspeccionDto>> ObtenerTodosAsync()
        {
            var inspecciones = await _repository.ObtenerTodosAsync();

            return inspecciones.Select(Mapear).ToList();
        }

        public async Task<InspeccionDto?> ObtenerPorIdAsync(int id)
        {
            var inspeccion = await _repository.ObtenerPorIdAsync(id);

            return inspeccion == null ? null : Mapear(inspeccion);
        }

        public async Task<InspeccionCatalogosDto> ObtenerCatalogosAsync()
        {
            var contratos = await _repository.ObtenerContratosAsync();
            var empleados = await _repository.ObtenerEmpleadosAsync();

            return new InspeccionCatalogosDto
            {
                Contratos = contratos.Select(c => new InspeccionOpcionDto
                {
                    Id = c.Id,
                    Nombre = $"{c.NumeroContrato} - " +
                             $"{c.Reserva?.Cliente?.Nombres} " +
                             $"{c.Reserva?.Cliente?.Apellidos} - " +
                             $"{c.Reserva?.Vehiculo?.Placa}"
                }).ToList(),

                Empleados = empleados.Select(e => new InspeccionOpcionDto
                {
                    Id = e.Id,
                    Nombre = $"{e.Nombres} {e.Apellidos} - {e.Cargo}"
                }).ToList()
            };
        }

        public async Task CrearAsync(InspeccionDto dto)
        {
            await ValidarAsync(dto);

            var inspeccion = new Inspeccion
            {
                ContratoId = dto.ContratoId,
                EmpleadoId = dto.EmpleadoId,
                Fecha = DateTime.UtcNow,
                Tipo = dto.Tipo.Trim(),
                Kilometraje = dto.Kilometraje,
                NivelCombustible = dto.NivelCombustible,
                Observaciones = Normalizar(dto.Observaciones)
            };

            await _repository.AgregarAsync(inspeccion);
        }

        public async Task ActualizarAsync(InspeccionDto dto)
        {
            var inspeccion = await _repository.ObtenerPorIdAsync(dto.Id);

            if (inspeccion == null)
            {
                throw new KeyNotFoundException(
                    "La inspección seleccionada no existe.");
            }

            if (inspeccion.ContratoId != dto.ContratoId ||
                inspeccion.Tipo != dto.Tipo)
            {
                throw new InvalidOperationException(
                    "No puedes cambiar el contrato ni el tipo de una inspección registrada.");
            }

            await ValidarAsync(dto, dto.Id);

            inspeccion.EmpleadoId = dto.EmpleadoId;
            inspeccion.Kilometraje = dto.Kilometraje;
            inspeccion.NivelCombustible = dto.NivelCombustible;
            inspeccion.Observaciones = Normalizar(dto.Observaciones);

            await _repository.GuardarCambiosAsync();
        }

        private async Task ValidarAsync(
            InspeccionDto dto,
            int excluirId = 0)
        {
            if (dto.Tipo != "Entrega" &&
                dto.Tipo != "Devolución")
            {
                throw new InvalidOperationException(
                    "Selecciona un tipo de inspección válido.");
            }

            if (dto.Kilometraje < 0)
            {
                throw new InvalidOperationException(
                    "El kilometraje no puede ser negativo.");
            }

            if (dto.NivelCombustible < 0 ||
                dto.NivelCombustible > 100)
            {
                throw new InvalidOperationException(
                    "El nivel de combustible debe estar entre 0 y 100.");
            }

            if (dto.Observaciones?.Trim().Length > 500)
            {
                throw new InvalidOperationException(
                    "Las observaciones no pueden superar los 500 caracteres.");
            }

            var contrato = await _repository.ObtenerContratoAsync(
                dto.ContratoId);

            if (contrato == null)
            {
                throw new InvalidOperationException(
                    "El contrato seleccionado no existe.");
            }

            if (contrato.Estado == "Anulado" ||
                contrato.Estado == "Cancelado")
            {
                throw new InvalidOperationException(
                    "No puedes inspeccionar un contrato anulado o cancelado.");
            }

            var empleado = await _repository.ObtenerEmpleadoAsync(
                dto.EmpleadoId);

            if (empleado == null || !empleado.Activo)
            {
                throw new InvalidOperationException(
                    "Selecciona un empleado activo.");
            }

            if (await _repository.ExisteTipoAsync(
                dto.ContratoId,
                dto.Tipo,
                excluirId))
            {
                throw new InvalidOperationException(
                    $"Este contrato ya tiene registrada una inspección de {dto.Tipo.ToLower()}.");
            }

            if (dto.Tipo == "Devolución")
            {
                var inspecciones = await _repository.ObtenerTodosAsync();

                var entrega = inspecciones.FirstOrDefault(i =>
                    i.ContratoId == dto.ContratoId &&
                    i.Tipo == "Entrega");

                if (entrega == null)
                {
                    throw new InvalidOperationException(
                        "Debes registrar primero la inspección de entrega.");
                }

                if (dto.Kilometraje < entrega.Kilometraje)
                {
                    throw new InvalidOperationException(
                        "El kilometraje de devolución no puede ser menor que el registrado en la entrega.");
                }
            }
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        private static InspeccionDto Mapear(Inspeccion inspeccion)
        {
            var contrato = inspeccion.Contrato;
            var reserva = contrato?.Reserva;

            return new InspeccionDto
            {
                Id = inspeccion.Id,
                ContratoId = inspeccion.ContratoId,
                EmpleadoId = inspeccion.EmpleadoId,
                Fecha = inspeccion.Fecha,
                Tipo = inspeccion.Tipo,
                Kilometraje = inspeccion.Kilometraje,
                NivelCombustible = inspeccion.NivelCombustible,
                Observaciones = inspeccion.Observaciones,
                NumeroContrato = contrato?.NumeroContrato ?? string.Empty,
                NombreCliente = reserva?.Cliente == null
                    ? string.Empty
                    : $"{reserva.Cliente.Nombres} {reserva.Cliente.Apellidos}",
                NombreVehiculo = reserva?.Vehiculo == null
                    ? string.Empty
                    : $"{reserva.Vehiculo.Placa} - {reserva.Vehiculo.Modelo?.Nombre}",
                NombreEmpleado = inspeccion.Empleado == null
                    ? string.Empty
                    : $"{inspeccion.Empleado.Nombres} {inspeccion.Empleado.Apellidos}"
            };
        }
    }
}