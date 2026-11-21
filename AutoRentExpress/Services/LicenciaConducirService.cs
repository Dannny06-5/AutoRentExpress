
using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;

namespace AutoRentExpress.Services
{
    public class LicenciaConducirService : ILicenciaConducirService
    {
        private readonly ILicenciaConducirRepository _repository;

        public LicenciaConducirService(
            ILicenciaConducirRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<LicenciaConducirDto>> ObtenerTodosAsync()
        {
            var licencias = await _repository.ObtenerTodosAsync();

            return licencias.Select(Mapear).ToList();
        }

        public async Task<LicenciaConducirDto?> ObtenerPorIdAsync(int id)
        {
            var licencia = await _repository.ObtenerPorIdAsync(id);

            return licencia == null ? null : Mapear(licencia);
        }

        public async Task<List<LicenciaClienteDto>> ObtenerClientesAsync()
        {
            var clientes = await _repository.ObtenerClientesAsync();

            return clientes.Select(c => new LicenciaClienteDto
            {
                Id = c.Id,
                Nombre = $"{c.Nombres} {c.Apellidos}"
            }).ToList();
        }

        public async Task CrearAsync(LicenciaConducirDto dto)
        {
            await ValidarAsync(dto);

            var licencia = new LicenciaConducir
            {
                ClienteId = dto.ClienteId,
                Numero = dto.Numero.Trim(),
                Categoria = dto.Categoria.Trim().ToUpperInvariant(),
                FechaEmision = dto.FechaEmision.Date,
                FechaVencimiento = dto.FechaVencimiento.Date,
                PaisEmision = dto.PaisEmision.Trim()
            };

            await _repository.AgregarAsync(licencia);
        }

        public async Task ActualizarAsync(LicenciaConducirDto dto)
        {
            var licencia = await _repository.ObtenerPorIdAsync(dto.Id);

            if (licencia == null)
            {
                throw new KeyNotFoundException(
                    "La licencia seleccionada no existe.");
            }

            await ValidarAsync(dto, dto.Id);

            licencia.ClienteId = dto.ClienteId;
            licencia.Numero = dto.Numero.Trim();
            licencia.Categoria = dto.Categoria.Trim().ToUpperInvariant();
            licencia.FechaEmision = dto.FechaEmision.Date;
            licencia.FechaVencimiento = dto.FechaVencimiento.Date;
            licencia.PaisEmision = dto.PaisEmision.Trim();

            await _repository.GuardarCambiosAsync();
        }

        private async Task ValidarAsync(
            LicenciaConducirDto dto,
            int excluirId = 0)
        {
            if (string.IsNullOrWhiteSpace(dto.Numero))
            {
                throw new InvalidOperationException(
                    "El número de licencia es obligatorio.");
            }

            if (dto.Numero.Trim().Length > 40)
            {
                throw new InvalidOperationException(
                    "El número de licencia no puede superar los 40 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(dto.Categoria))
            {
                throw new InvalidOperationException(
                    "La categoría de licencia es obligatoria.");
            }

            if (dto.Categoria.Trim().Length > 20)
            {
                throw new InvalidOperationException(
                    "La categoría no puede superar los 20 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaisEmision))
            {
                throw new InvalidOperationException(
                    "El país de emisión es obligatorio.");
            }

            if (dto.PaisEmision.Trim().Length > 100)
            {
                throw new InvalidOperationException(
                    "El país de emisión no puede superar los 100 caracteres.");
            }

            if (dto.FechaEmision == default ||
                dto.FechaVencimiento == default)
            {
                throw new InvalidOperationException(
                    "Selecciona ambas fechas de la licencia.");
            }

            if (dto.FechaEmision.Date > DateTime.Today)
            {
                throw new InvalidOperationException(
                    "La fecha de emisión no puede ser futura.");
            }

            if (dto.FechaVencimiento.Date <= dto.FechaEmision.Date)
            {
                throw new InvalidOperationException(
                    "La fecha de vencimiento debe ser posterior a la fecha de emisión.");
            }

            var cliente = await _repository.ObtenerClienteAsync(dto.ClienteId);

            if (cliente == null)
            {
                throw new InvalidOperationException(
                    "El cliente seleccionado no existe.");
            }

            if (await _repository.ExisteNumeroAsync(
                dto.Numero,
                excluirId))
            {
                throw new InvalidOperationException(
                    "Ya existe una licencia registrada con ese número.");
            }
        }

        private static LicenciaConducirDto Mapear(LicenciaConducir licencia)
        {
            return new LicenciaConducirDto
            {
                Id = licencia.Id,
                ClienteId = licencia.ClienteId,
                Numero = licencia.Numero,
                Categoria = licencia.Categoria,
                FechaEmision = licencia.FechaEmision,
                FechaVencimiento = licencia.FechaVencimiento,
                PaisEmision = licencia.PaisEmision,
                NombreCliente = licencia.Cliente == null
                    ? string.Empty
                    : $"{licencia.Cliente.Nombres} {licencia.Cliente.Apellidos}"
            };
        }
    }
}
