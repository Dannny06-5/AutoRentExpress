using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Services
{
    public class ModeloService : IModeloService
    {
        private readonly IModeloRepository _repository;
        private readonly IMarcaRepository _marcas;

        public ModeloService(
            IModeloRepository repository,
            IMarcaRepository marcas)
        {
            _repository = repository;
            _marcas = marcas;
        }

        public async Task<List<ModeloDto>> ObtenerTodosAsync()
        {
            var modelos = await _repository.ObtenerTodosAsync();

            return modelos.Select(ConvertirDto).ToList();
        }

        public async Task<ModeloDto?> ObtenerPorIdAsync(int id)
        {
            var modelo = await _repository.ObtenerPorIdAsync(id);

            return modelo == null ? null : ConvertirDto(modelo);
        }

        public async Task CrearAsync(ModeloDto dto)
        {
            Validar(dto);

            if (await _marcas.ObtenerPorIdAsync(dto.MarcaId) == null)
                throw new InvalidOperationException("La marca seleccionada no existe.");

            var nombre = dto.Nombre.Trim();

            if (await _repository.ExisteAsync(nombre, dto.MarcaId))
                throw new InvalidOperationException(
                    "Este modelo ya existe para la marca seleccionada.");

            await _repository.AgregarAsync(new Modelo
            {
                Nombre = nombre,
                MarcaId = dto.MarcaId
            });

            await _repository.GuardarCambiosAsync();
        }

        public async Task ActualizarAsync(ModeloDto dto)
        {
            Validar(dto);

            var modelo = await _repository.ObtenerPorIdAsync(dto.Id);

            if (modelo == null)
                throw new KeyNotFoundException("El modelo no existe.");

            if (await _marcas.ObtenerPorIdAsync(dto.MarcaId) == null)
                throw new InvalidOperationException("La marca seleccionada no existe.");

            var nombre = dto.Nombre.Trim();

            if (await _repository.ExisteAsync(nombre, dto.MarcaId, dto.Id))
                throw new InvalidOperationException(
                    "Este modelo ya existe para la marca seleccionada.");

            modelo.Nombre = nombre;
            modelo.MarcaId = dto.MarcaId;

            await _repository.GuardarCambiosAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var modelo = await _repository.ObtenerPorIdAsync(id);

            if (modelo == null)
                throw new KeyNotFoundException("El modelo no existe.");

            if (await _repository.TieneVehiculosAsync(id))
                throw new InvalidOperationException(
                    "No se puede eliminar un modelo con vehículos asociados.");

            _repository.Eliminar(modelo);
            await _repository.GuardarCambiosAsync();
        }

        private static ModeloDto ConvertirDto(Modelo modelo)
        {
            return new ModeloDto
            {
                Id = modelo.Id,
                Nombre = modelo.Nombre,
                MarcaId = modelo.MarcaId,
                NombreMarca = modelo.Marca?.Nombre ?? string.Empty
            };
        }

        private static void Validar(ModeloDto dto)
        {
            Validator.ValidateObject(dto, new ValidationContext(dto), true);

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ValidationException("El nombre es obligatorio.");
        }
    }
}