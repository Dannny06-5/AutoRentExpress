using AutoRentExpress.DTOs;
using AutoRentExpress.Models;
using AutoRentExpress.Repositories;

namespace AutoRentExpress.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _repository;

        public EmpleadoService(IEmpleadoRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EmpleadoDto>> ObtenerTodosAsync()
        {
            var empleados = await _repository.ObtenerTodosAsync();

            return empleados.Select(e => new EmpleadoDto
            {
                Id = e.Id,
                SucursalId = e.SucursalId,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                Cargo = e.Cargo,
                Telefono = e.Telefono,
                Activo = e.Activo,
                NombreSucursal = e.Sucursal?.Nombre ?? string.Empty
            }).ToList();
        }

        public async Task<EmpleadoDto?> ObtenerPorIdAsync(int id)
        {
            var empleado = await _repository.ObtenerPorIdAsync(id);

            if (empleado == null)
                return null;

            return new EmpleadoDto
            {
                Id = empleado.Id,
                SucursalId = empleado.SucursalId,
                Nombres = empleado.Nombres,
                Apellidos = empleado.Apellidos,
                Cargo = empleado.Cargo,
                Telefono = empleado.Telefono,
                Activo = empleado.Activo
            };
        }

        public async Task<List<EmpleadoSucursalDto>> ObtenerSucursalesAsync()
        {
            var sucursales = await _repository.ObtenerSucursalesAsync();

            return sucursales
                .Select(s => new EmpleadoSucursalDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre
                })
                .ToList();
        }

        public async Task CrearAsync(EmpleadoDto dto)
        {
            await ValidarSucursalAsync(dto.SucursalId);

            var empleado = new Empleado
            {
                SucursalId = dto.SucursalId,
                Nombres = dto.Nombres.Trim(),
                Apellidos = dto.Apellidos.Trim(),
                Cargo = dto.Cargo.Trim(),
                Telefono = Normalizar(dto.Telefono),
                Activo = true
            };

            await _repository.AgregarAsync(empleado);
        }

        public async Task ActualizarAsync(EmpleadoDto dto)
        {
            var empleado = await _repository.ObtenerPorIdAsync(dto.Id);

            if (empleado == null)
                throw new KeyNotFoundException("El empleado no existe.");

            await ValidarSucursalAsync(dto.SucursalId);

            empleado.SucursalId = dto.SucursalId;
            empleado.Nombres = dto.Nombres.Trim();
            empleado.Apellidos = dto.Apellidos.Trim();
            empleado.Cargo = dto.Cargo.Trim();
            empleado.Telefono = Normalizar(dto.Telefono);

            await _repository.GuardarCambiosAsync();
        }

        public async Task CambiarEstadoActivoAsync(int id)
        {
            var empleado = await _repository.ObtenerPorIdAsync(id);

            if (empleado == null)
                throw new KeyNotFoundException("El empleado no existe.");

            if (!empleado.Activo)
            {
                await ValidarSucursalAsync(empleado.SucursalId);
            }

            empleado.Activo = !empleado.Activo;

            await _repository.GuardarCambiosAsync();
        }

        private async Task ValidarSucursalAsync(int sucursalId)
        {
            var sucursal = await _repository.ObtenerSucursalAsync(sucursalId);

            if (sucursal == null)
                throw new InvalidOperationException(
                    "La sucursal seleccionada no existe.");

            if (!sucursal.Activa)
                throw new InvalidOperationException(
                    "La sucursal seleccionada está inactiva.");
        }

        private static string? Normalizar(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? null
                : valor.Trim();
        }
    }
}