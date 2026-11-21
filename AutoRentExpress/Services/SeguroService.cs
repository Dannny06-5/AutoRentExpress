using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Services
{
    public class SeguroService : ISeguroService
    {
        private readonly ISeguroRepository _repository;

        public SeguroService(ISeguroRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SeguroDto>> ObtenerTodosAsync()
        {
            var seguros = await _repository.ObtenerTodosAsync();

            return seguros.Select(Mapear).ToList();
        }

        public async Task<SeguroDto?> ObtenerPorIdAsync(int id)
        {
            var seguro = await _repository.ObtenerPorIdAsync(id);

            return seguro == null ? null : Mapear(seguro);
        }

        public async Task CrearAsync(SeguroDto dto)
        {
            await ValidarAsync(dto);

            var seguro = new Seguro
            {
                Nombre = dto.Nombre.Trim(),
                Cobertura = Normalizar(dto.Cobertura),
                CostoDiario = dto.CostoDiario,
                Activo = true
            };

            await _repository.AgregarAsync(seguro);
        }

        public async Task ActualizarAsync(SeguroDto dto)
        {
            var seguro = await _repository.ObtenerPorIdAsync(dto.Id);

            if (seguro == null)
            {
                throw new KeyNotFoundException(
                    "El seguro seleccionado no existe.");
            }

            await ValidarAsync(dto, dto.Id);

            seguro.Nombre = dto.Nombre.Trim();
            seguro.Cobertura = Normalizar(dto.Cobertura);
            seguro.CostoDiario = dto.CostoDiario;

            await _repository.GuardarCambiosAsync();
        }

        public async Task CambiarEstadoActivoAsync(int id)
        {
            var seguro = await _repository.ObtenerPorIdAsync(id);

            if (seguro == null)
            {
                throw new KeyNotFoundException(
                    "El seguro seleccionado no existe.");
            }

            seguro.Activo = !seguro.Activo;

            await _repository.GuardarCambiosAsync();
        }

        private async Task ValidarAsync(
            SeguroDto dto,
            int excluirId = 0)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
            {
                throw new ValidationException(
                    "El nombre del seguro es obligatorio.");
            }

            if (dto.Nombre.Trim().Length > 100)
            {
                throw new ValidationException(
                    "El nombre no puede superar los 100 caracteres.");
            }

            if (dto.Cobertura?.Trim().Length > 500)
            {
                throw new ValidationException(
                    "La cobertura no puede superar los 500 caracteres.");
            }

            if (dto.CostoDiario < 0 ||
                dto.CostoDiario > 1000000)
            {
                throw new ValidationException(
                    "El costo diario debe estar entre 0 y 1000000.");
            }

            if (await _repository.ExisteNombreAsync(
                dto.Nombre,
                excluirId))
            {
                throw new InvalidOperationException(
                    "Ya existe un seguro registrado con ese nombre.");
            }
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        private static SeguroDto Mapear(Seguro seguro)
        {
            return new SeguroDto
            {
                Id = seguro.Id,
                Nombre = seguro.Nombre,
                Cobertura = seguro.Cobertura,
                CostoDiario = seguro.CostoDiario,
                Activo = seguro.Activo
            };
        }
    }
}