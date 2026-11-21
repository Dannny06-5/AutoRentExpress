using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Services
{
    public class EstadoVehiculoService : IEstadoVehiculoService
    {
        private readonly IEstadoVehiculoRepository _repository;

        public EstadoVehiculoService(IEstadoVehiculoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EstadoVehiculoDto>> ObtenerTodosAsync()
        {
            var estados = await _repository.ObtenerTodosAsync();

            return estados.Select(ConvertirDto).ToList();
        }

        public async Task<EstadoVehiculoDto?> ObtenerPorIdAsync(int id)
        {
            var estado = await _repository.ObtenerPorIdAsync(id);

            return estado == null ? null : ConvertirDto(estado);
        }

        public async Task CrearAsync(EstadoVehiculoDto dto)
        {
            Validar(dto);

            var nombre = dto.Nombre.Trim();

            if (await _repository.ExisteNombreAsync(nombre))
                throw new InvalidOperationException(
                    "Ya existe un estado de vehículo con ese nombre.");

            await _repository.AgregarAsync(new EstadoVehiculo
            {
                Nombre = nombre
            });

            await _repository.GuardarCambiosAsync();
        }

        public async Task ActualizarAsync(EstadoVehiculoDto dto)
        {
            Validar(dto);

            var estado = await _repository.ObtenerPorIdAsync(dto.Id);

            if (estado == null)
                throw new KeyNotFoundException("El estado no existe.");

            var nombre = dto.Nombre.Trim();

            if (await _repository.ExisteNombreAsync(nombre, dto.Id))
                throw new InvalidOperationException(
                    "Ya existe otro estado con ese nombre.");

            estado.Nombre = nombre;

            await _repository.GuardarCambiosAsync();
        }

        public async Task EliminarAsync(int id)
        {
            var estado = await _repository.ObtenerPorIdAsync(id);

            if (estado == null)
                throw new KeyNotFoundException("El estado no existe.");

            if (await _repository.TieneVehiculosAsync(id))
                throw new InvalidOperationException(
                    "No se puede eliminar un estado que tiene vehículos asociados.");

            _repository.Eliminar(estado);
            await _repository.GuardarCambiosAsync();
        }

        private static EstadoVehiculoDto ConvertirDto(EstadoVehiculo estado)
        {
            return new EstadoVehiculoDto
            {
                Id = estado.Id,
                Nombre = estado.Nombre
            };
        }

        private static void Validar(EstadoVehiculoDto dto)
        {
            Validator.ValidateObject(
                dto,
                new ValidationContext(dto),
                true);

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ValidationException(
                    "El nombre del estado es obligatorio.");
        }
    }
}