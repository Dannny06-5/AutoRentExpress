using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;
using System.ComponentModel.DataAnnotations;

namespace AutoRentExpress.Services
{
    public class VehiculoService : IVehiculoService
    {
        private readonly IVehiculoRepository _repository;

        public VehiculoService(IVehiculoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<VehiculoDto>> ObtenerTodosAsync()
        {
            var vehiculos = await _repository.ObtenerTodosAsync();
            return vehiculos.Select(ConvertirDto).ToList();
        }

        public async Task<VehiculoDto?> ObtenerPorIdAsync(int id)
        {
            var vehiculo = await _repository.ObtenerPorIdAsync(id);
            return vehiculo == null ? null : ConvertirDto(vehiculo);
        }

        public async Task<VehiculoCatalogosDto> ObtenerCatalogosAsync()
        {
            var datos = await _repository.ObtenerCatalogosAsync();

            return new VehiculoCatalogosDto
            {
                Modelos = datos.Modelos.Select(m => new OpcionVehiculoDto
                {
                    Id = m.Id,
                    Nombre = $"{m.Marca.Nombre} - {m.Nombre}"
                }).ToList(),

                Categorias = datos.Categorias.Select(c => new OpcionVehiculoDto
                {
                    Id = c.Id,
                    Nombre = c.Nombre
                }).ToList(),

                Estados = datos.Estados.Select(e => new OpcionVehiculoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                }).ToList(),

                Sucursales = datos.Sucursales.Select(s => new OpcionVehiculoDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre
                }).ToList()
            };
        }

        public async Task CrearAsync(VehiculoDto dto)
        {
            Validar(dto);
            await ValidarCatalogos(dto);

            var placa = dto.Placa.Trim().ToUpperInvariant();

            if (await _repository.ExistePlacaAsync(placa))
                throw new InvalidOperationException(
                    "Ya existe un vehículo registrado con esa placa.");

            var vehiculo = new Vehiculo
            {
                Placa = placa,
                ModeloId = dto.ModeloId,
                CategoriaVehiculoId = dto.CategoriaVehiculoId,
                EstadoVehiculoId = dto.EstadoVehiculoId,
                SucursalId = dto.SucursalId,
                Color = dto.Color.Trim(),
                Anio = dto.Anio,
                Kilometraje = dto.Kilometraje,
                CapacidadPasajeros = dto.CapacidadPasajeros,
                TarifaDiaria = dto.TarifaDiaria,
                ImagenUrl = Normalizar(dto.ImagenUrl),
                Activo = true
            };

            await _repository.AgregarAsync(vehiculo);
            await _repository.GuardarCambiosAsync();
        }

        public async Task ActualizarAsync(VehiculoDto dto)
        {
            Validar(dto);
            await ValidarCatalogos(dto);

            var vehiculo = await _repository.ObtenerPorIdAsync(dto.Id);

            if (vehiculo == null)
                throw new KeyNotFoundException("El vehículo no existe.");

            var placa = dto.Placa.Trim().ToUpperInvariant();

            if (await _repository.ExistePlacaAsync(placa, dto.Id))
                throw new InvalidOperationException(
                    "Ya existe otro vehículo con esa placa.");

            vehiculo.Placa = placa;
            vehiculo.ModeloId = dto.ModeloId;
            vehiculo.CategoriaVehiculoId = dto.CategoriaVehiculoId;
            vehiculo.EstadoVehiculoId = dto.EstadoVehiculoId;
            vehiculo.SucursalId = dto.SucursalId;
            vehiculo.Color = dto.Color.Trim();
            vehiculo.Anio = dto.Anio;
            vehiculo.Kilometraje = dto.Kilometraje;
            vehiculo.CapacidadPasajeros = dto.CapacidadPasajeros;
            vehiculo.TarifaDiaria = dto.TarifaDiaria;
            vehiculo.ImagenUrl = Normalizar(dto.ImagenUrl);

            await _repository.GuardarCambiosAsync();
        }

        public async Task CambiarEstadoActivoAsync(int id)
        {
            var vehiculo = await _repository.ObtenerPorIdAsync(id);

            if (vehiculo == null)
                throw new KeyNotFoundException("El vehículo no existe.");

            vehiculo.Activo = !vehiculo.Activo;
            await _repository.GuardarCambiosAsync();
        }

        private async Task ValidarCatalogos(VehiculoDto dto)
        {
            var datos = await _repository.ObtenerCatalogosAsync();

            if (!datos.Modelos.Any(m => m.Id == dto.ModeloId))
                throw new InvalidOperationException("Selecciona un modelo válido.");

            if (!datos.Categorias.Any(c => c.Id == dto.CategoriaVehiculoId))
                throw new InvalidOperationException("Selecciona una categoría válida.");

            if (!datos.Estados.Any(e => e.Id == dto.EstadoVehiculoId))
                throw new InvalidOperationException("Selecciona un estado válido.");

            if (!datos.Sucursales.Any(s => s.Id == dto.SucursalId))
                throw new InvalidOperationException("Selecciona una sucursal activa.");
        }

        private static VehiculoDto ConvertirDto(Vehiculo vehiculo)
        {
            return new VehiculoDto
            {
                Id = vehiculo.Id,
                Placa = vehiculo.Placa,
                ModeloId = vehiculo.ModeloId,
                CategoriaVehiculoId = vehiculo.CategoriaVehiculoId,
                EstadoVehiculoId = vehiculo.EstadoVehiculoId,
                SucursalId = vehiculo.SucursalId,
                Color = vehiculo.Color,
                Anio = vehiculo.Anio,
                Kilometraje = vehiculo.Kilometraje,
                CapacidadPasajeros = vehiculo.CapacidadPasajeros,
                TarifaDiaria = vehiculo.TarifaDiaria,
                ImagenUrl = vehiculo.ImagenUrl,
                Activo = vehiculo.Activo,
                NombreModelo = vehiculo.Modelo == null
                    ? string.Empty
                    : $"{vehiculo.Modelo.Marca?.Nombre} {vehiculo.Modelo.Nombre}",
                NombreCategoria = vehiculo.CategoriaVehiculo?.Nombre,
                NombreEstado = vehiculo.EstadoVehiculo?.Nombre,
                NombreSucursal = vehiculo.Sucursal?.Nombre
            };
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }

        private static void Validar(VehiculoDto dto)
        {
            Validator.ValidateObject(dto, new ValidationContext(dto), true);

            if (string.IsNullOrWhiteSpace(dto.Placa) ||
                string.IsNullOrWhiteSpace(dto.Color))
                throw new ValidationException(
                    "La placa y el color son obligatorios.");
        }
    }
}