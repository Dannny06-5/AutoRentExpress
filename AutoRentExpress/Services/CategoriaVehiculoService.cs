using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Services
{
    public class CategoriaVehiculoService : ICategoriaVehiculoService
    {
        private readonly ICategoriaVehiculoRepository _repository;

        public CategoriaVehiculoService(ICategoriaVehiculoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CategoriaVehiculoDto>> ObtenerTodosAsync()
        {
            var categorias = await _repository.ObtenerTodosAsync();

            return categorias.Select(c => ConvertirDto(c)).ToList();
        }

        public async Task<CategoriaVehiculoDto?> ObtenerPorIdAsync(int id)
        {
            var categoria = await _repository.ObtenerPorIdAsync(id);

            return categoria == null ? null : ConvertirDto(categoria);
        }

        public async Task CrearAsync(CategoriaVehiculoDto dto)
        {
            Validar(dto);

            var nombre = dto.Nombre.Trim();

            if (await _repository.ExisteNombreAsync(nombre))
                throw new InvalidOperationException("La categoría ya existe.");

            var categoria = new CategoriaVehiculo
            {
                Nombre = nombre,
                Descripcion = dto.Descripcion?.Trim() ?? string.Empty
            };

            await _repository.AgregarAsync(categoria);
            await _repository.GuardarCambiosAsync();
        }

        public async Task ActualizarAsync(CategoriaVehiculoDto dto)
        {
            Validar(dto);

            var categoria = await _repository.ObtenerPorIdAsync(dto.Id);

            if (categoria == null)
                throw new KeyNotFoundException("La categoría no existe.");

            var nombre = dto.Nombre.Trim();

            if (await _repository.ExisteNombreAsync(nombre, dto.Id))
                throw new InvalidOperationException("La categoría ya existe.");

            categoria.Nombre = nombre;
            categoria.Descripcion = dto.Descripcion?.Trim() ?? string.Empty;

            await _repository.GuardarCambiosAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var categoria = await _repository.ObtenerPorIdAsync(id);

            if (categoria == null)
                throw new KeyNotFoundException("La categoría no existe.");

            if (await _repository.TieneRelacionesAsync(id))
                throw new InvalidOperationException(
                    "No se puede eliminar una categoría que tiene vehículos o tarifas asociados.");

            _repository.Eliminar(categoria);
            await _repository.GuardarCambiosAsync();
        }

        private static CategoriaVehiculoDto ConvertirDto(CategoriaVehiculo categoria)
        {
            return new CategoriaVehiculoDto
            {
                Id = categoria.Id,
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion ?? string.Empty
            };
        }

        private static void Validar(CategoriaVehiculoDto dto)
        {
            Validator.ValidateObject(dto, new ValidationContext(dto), true);

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ValidationException("El nombre es obligatorio.");
        }
    }
}