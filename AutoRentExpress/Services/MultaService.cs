using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;

namespace AutoRentExpress.Services
{
    public class MultaService : IMultaService
    {
        private readonly IMultaRepository _repository;

        public MultaService(IMultaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MultaDto>> ObtenerTodosAsync()
        {
            var multas = await _repository.ObtenerTodosAsync();
            return multas.Select(Mapear).ToList();
        }

        public async Task<MultaDto?> ObtenerPorIdAsync(int id)
        {
            var multa = await _repository.ObtenerPorIdAsync(id);
            return multa == null ? null : Mapear(multa);
        }

        public async Task<List<MultaContratoDto>> ObtenerContratosAsync()
        {
            var contratos = await _repository.ObtenerContratosAsync();

            return contratos.Select(c => new MultaContratoDto
            {
                Id = c.Id,
                NumeroContrato = c.NumeroContrato,
                NombreCliente = c.Reserva?.Cliente == null
                    ? string.Empty
                    : $"{c.Reserva.Cliente.Nombres} {c.Reserva.Cliente.Apellidos}"
            }).ToList();
        }

        public async Task CrearAsync(MultaDto dto)
        {
            ValidarDatos(dto);

            var contrato = await _repository.ObtenerContratoAsync(dto.ContratoId);

            if (contrato == null)
                throw new InvalidOperationException(
                    "El contrato seleccionado no existe.");

            if (contrato.Estado == "Anulado" ||
                contrato.Estado == "Cancelado")
            {
                throw new InvalidOperationException(
                    "No puedes registrar multas en un contrato anulado o cancelado.");
            }

            var multa = new Multa
            {
                ContratoId = contrato.Id,
                Motivo = dto.Motivo.Trim(),
                Monto = dto.Monto,
                Fecha = DateTime.UtcNow,
                Pagada = false
            };

            await _repository.AgregarAsync(multa);
        }

        public async Task ActualizarAsync(MultaDto dto)
        {
            var multa = await _repository.ObtenerPorIdAsync(dto.Id);

            if (multa == null)
                throw new KeyNotFoundException("La multa no existe.");

            if (multa.Pagada)
                throw new InvalidOperationException(
                    "No puedes modificar una multa que ya fue pagada.");

            ValidarDatos(dto);

            if (multa.ContratoId != dto.ContratoId)
                throw new InvalidOperationException(
                    "No puedes cambiar el contrato asociado a una multa.");

            multa.Motivo = dto.Motivo.Trim();
            multa.Monto = dto.Monto;

            await _repository.GuardarCambiosAsync();
        }

        public async Task MarcarPagadaAsync(int id)
        {
            var multa = await _repository.ObtenerPorIdAsync(id);

            if (multa == null)
                throw new KeyNotFoundException("La multa no existe.");

            if (multa.Pagada)
                throw new InvalidOperationException(
                    "Esta multa ya está marcada como pagada.");

            multa.Pagada = true;

            await _repository.GuardarCambiosAsync();
        }

        private static void ValidarDatos(MultaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Motivo))
                throw new InvalidOperationException(
                    "El motivo de la multa es obligatorio.");

            if (dto.Motivo.Trim().Length > 100)
                throw new InvalidOperationException(
                    "El motivo no puede superar los 100 caracteres.");

            if (dto.Monto <= 0 || dto.Monto > 100000000m)
                throw new InvalidOperationException(
                    "El monto de la multa debe ser mayor que cero y no superar 100000000.");
        }

        private static MultaDto Mapear(Multa multa)
        {
            return new MultaDto
            {
                Id = multa.Id,
                ContratoId = multa.ContratoId,
                Motivo = multa.Motivo,
                Monto = multa.Monto,
                Fecha = multa.Fecha,
                Pagada = multa.Pagada,
                NumeroContrato = multa.Contrato?.NumeroContrato
                    ?? string.Empty,
                NombreCliente = multa.Contrato?.Reserva?.Cliente == null
                    ? string.Empty
                    : $"{multa.Contrato.Reserva.Cliente.Nombres} " +
                      $"{multa.Contrato.Reserva.Cliente.Apellidos}"
            };
        }
    }
}