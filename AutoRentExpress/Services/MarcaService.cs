using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _repository;

        public MarcaService(IMarcaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MarcaDto>> ObtenerTodosAsync()
        {
            var marcas = await _repository.ObtenerTodosAsync();

            return marcas.Select(m => new MarcaDto
            {
                Id = m.Id,
                Nombre = m.Nombre
            }).ToList();
        }

        public async Task<MarcaDto?> ObtenerPorIdAsync(int id)
        {
            var marca = await _repository.ObtenerPorIdAsync(id);

            if (marca == null)
                return null;

            return new MarcaDto
            {
                Id = marca.Id,
                Nombre = marca.Nombre
            };
        }

        public async Task CrearAsync(MarcaDto dto)
        {
            Validar(dto);

            var nombre = dto.Nombre.Trim();

            if (await _repository.ExisteNombreAsync(nombre))
                throw new InvalidOperationException("La marca ya existe.");

            await _repository.AgregarAsync(new Marca
            {
                Nombre = nombre
            });

            await _repository.GuardarCambiosAsync();
        }

        public async Task ActualizarAsync(MarcaDto dto)
        {
            Validar(dto);

            var marca = await _repository.ObtenerPorIdAsync(dto.Id);

            if (marca == null)
                throw new KeyNotFoundException("La marca no existe.");

            var nombre = dto.Nombre.Trim();

            if (await _repository.ExisteNombreAsync(nombre, dto.Id))
                throw new InvalidOperationException("La marca ya existe.");

            marca.Nombre = nombre;

            await _repository.GuardarCambiosAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var marca = await _repository.ObtenerPorIdAsync(id);

            if (marca == null)
                throw new KeyNotFoundException("La marca no existe.");

            if (await _repository.TieneModelosAsync(id))
                throw new InvalidOperationException(
                    "No se puede eliminar una marca con modelos asociados.");

            _repository.Eliminar(marca);
            await _repository.GuardarCambiosAsync();
        }

        private static void Validar(MarcaDto dto)
        {
            Validator.ValidateObject(dto, new ValidationContext(dto), true);

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ValidationException("El nombre es obligatorio.");
        }
    }
}