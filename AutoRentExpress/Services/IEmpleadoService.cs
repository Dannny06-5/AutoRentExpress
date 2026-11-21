using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IEmpleadoService
    {
        Task<List<EmpleadoDto>> ObtenerTodosAsync();

        Task<EmpleadoDto?> ObtenerPorIdAsync(int id);

        Task<List<EmpleadoSucursalDto>> ObtenerSucursalesAsync();

        Task CrearAsync(EmpleadoDto dto);

        Task ActualizarAsync(EmpleadoDto dto);

        Task CambiarEstadoActivoAsync(int id);
    }
}