using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;

namespace AutoRentExpress.Services
{
    public class PagoService : IPagoService
    {
        private readonly IPagoRepository _repository;

        public PagoService(IPagoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PagoDto>> ObtenerTodosAsync()
        {
            var pagos = await _repository.ObtenerTodosAsync();
            var resultado = new List<PagoDto>();

            foreach (var pago in pagos)
            {
                resultado.Add(await MapearAsync(pago));
            }

            return resultado;
        }

        public async Task<PagoDto?> ObtenerPorIdAsync(int id)
        {
            var pago = await _repository.ObtenerPorIdAsync(id);

            return pago == null
                ? null
                : await MapearAsync(pago);
        }

        public async Task<List<PagoContratoDto>> ObtenerContratosAsync()
        {
            var contratos = await _repository.ObtenerContratosAsync();
            var resultado = new List<PagoContratoDto>();

            foreach (var contrato in contratos)
            {
                var pagado = await _repository.ObtenerTotalPagadoAsync(
                    contrato.Id);

                resultado.Add(new PagoContratoDto
                {
                    Id = contrato.Id,
                    NumeroContrato = contrato.NumeroContrato,
                    NombreCliente = contrato.Reserva?.Cliente == null
                        ? string.Empty
                        : $"{contrato.Reserva.Cliente.Nombres} {contrato.Reserva.Cliente.Apellidos}",
                    TotalContrato = contrato.TotalContrato,
                    TotalPagado = pagado,
                    SaldoPendiente = Math.Max(
                        0m,
                        contrato.TotalContrato - pagado)
                });
            }

            return resultado;
        }

        public async Task RegistrarAsync(PagoDto dto)
        {
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
                    "No se pueden registrar pagos a un contrato anulado o cancelado.");
            }

            if (dto.Monto <= 0)
            {
                throw new InvalidOperationException(
                    "El monto del pago debe ser mayor que cero.");
            }

            if (dto.Monto > 100000000m)
            {
                throw new InvalidOperationException(
                    "El monto del pago supera el máximo permitido.");
            }

            var metodo = dto.Metodo?.Trim() ?? string.Empty;

            if (metodo != "Efectivo" &&
                metodo != "Tarjeta" &&
                metodo != "Transferencia")
            {
                throw new InvalidOperationException(
                    "Selecciona un método de pago válido.");
            }

            var referencia = string.IsNullOrWhiteSpace(dto.Referencia)
                ? null
                : dto.Referencia.Trim();

            if (referencia?.Length > 100)
            {
                throw new InvalidOperationException(
                    "La referencia no puede superar los 100 caracteres.");
            }

            if (metodo != "Efectivo" &&
                string.IsNullOrWhiteSpace(referencia))
            {
                throw new InvalidOperationException(
                    "Debes ingresar una referencia para pagos con tarjeta o transferencia.");
            }

            if (!string.IsNullOrWhiteSpace(referencia) &&
                await _repository.ExisteReferenciaAsync(referencia, metodo))
            {
                throw new InvalidOperationException(
                    "Ya existe un pago confirmado con ese método y referencia.");
            }

            var totalPagado = await _repository.ObtenerTotalPagadoAsync(
                contrato.Id);

            var saldoPendiente = contrato.TotalContrato - totalPagado;

            if (saldoPendiente <= 0)
            {
                throw new InvalidOperationException(
                    "El contrato ya está completamente pagado.");
            }

            if (dto.Monto > saldoPendiente)
            {
                throw new InvalidOperationException(
                    $"El monto supera el saldo pendiente de {saldoPendiente:N2}.");
            }

            var pago = new Pago
            {
                ContratoId = contrato.Id,
                FechaPago = DateTime.UtcNow,
                Monto = dto.Monto,
                Metodo = metodo,
                Referencia = referencia,
                Estado = "Confirmado"
            };

            await _repository.AgregarAsync(pago);
        }

        private async Task<PagoDto> MapearAsync(Pago pago)
        {
            var totalPagado = await _repository.ObtenerTotalPagadoAsync(
                pago.ContratoId);

            var contrato = pago.Contrato;

            return new PagoDto
            {
                Id = pago.Id,
                ContratoId = pago.ContratoId,
                FechaPago = pago.FechaPago,
                Monto = pago.Monto,
                Metodo = pago.Metodo,
                Referencia = pago.Referencia,
                Estado = pago.Estado,
                NumeroContrato = contrato.NumeroContrato,
                NombreCliente = contrato.Reserva?.Cliente == null
                    ? string.Empty
                    : $"{contrato.Reserva.Cliente.Nombres} {contrato.Reserva.Cliente.Apellidos}",
                TotalContrato = contrato.TotalContrato,
                TotalPagado = totalPagado,
                SaldoPendiente = Math.Max(
                    0m,
                    contrato.TotalContrato - totalPagado)
            };
        }
    }
}