using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Services
{
    public class SucursalService : ISucursalService
    {
        private readonly ISucursalRepository _repository;

        public SucursalService(ISucursalRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SucursalDto>> ObtenerTodosAsync()
        {
            var sucursales = await _repository.ObtenerTodosAsync();

            return sucursales.Select(ConvertirDto).ToList();
        }

        public async Task<SucursalDto?> ObtenerPorIdAsync(int id)
        {
            var sucursal = await _repository.ObtenerPorIdAsync(id);

            return sucursal == null ? null : ConvertirDto(sucursal);
        }

        public async Task CrearAsync(SucursalDto dto)
        {
            Validar(dto);

            var nombre = dto.Nombre.Trim();

            if (await _repository.ExisteNombreAsync(nombre))
                throw new InvalidOperationException("La sucursal ya existe.");

            await _repository.AgregarAsync(new Sucursal
            {
                Nombre = nombre,
                Direccion = dto.Direccion.Trim(),
                Telefono = Normalizar(dto.Telefono),
                Activa = true
            });

            await _repository.GuardarCambiosAsync();
        }

        public async Task ActualizarAsync(SucursalDto dto)
        {
            Validar(dto);

            var sucursal = await _repository.ObtenerPorIdAsync(dto.Id);

            if (sucursal == null)
                throw new KeyNotFoundException("La sucursal no existe.");

            var nombre = dto.Nombre.Trim();

            if (await _repository.ExisteNombreAsync(nombre, dto.Id))
                throw new InvalidOperationException("La sucursal ya existe.");

            sucursal.Nombre = nombre;
            sucursal.Direccion = dto.Direccion.Trim();
            sucursal.Telefono = Normalizar(dto.Telefono);

            await _repository.GuardarCambiosAsync();
        }

        public async Task CambiarEstadoAsync(int id)
        {
            var sucursal = await _repository.ObtenerPorIdAsync(id);

            if (sucursal == null)
                throw new KeyNotFoundException("La sucursal no existe.");

            sucursal.Activa = !sucursal.Activa;

            await _repository.GuardarCambiosAsync();
        }

        private static SucursalDto ConvertirDto(Sucursal sucursal)
        {
            return new SucursalDto
            {
                Id = sucursal.Id,
                Nombre = sucursal.Nombre,
                Direccion = sucursal.Direccion,
                Telefono = sucursal.Telefono,
                Activa = sucursal.Activa
            };
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }

        private static void Validar(SucursalDto dto)
        {
            Validator.ValidateObject(dto, new ValidationContext(dto), true);

            if (string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Direccion))
            {
                throw new ValidationException(
                    "El nombre y la dirección son obligatorios.");
            }
        }
    }
}