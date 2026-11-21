using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface ISucursalService
    {
        Task<List<SucursalDto>> ObtenerTodosAsync();
        Task<SucursalDto?> ObtenerPorIdAsync(int id);
        Task CrearAsync(SucursalDto dto);
        Task ActualizarAsync(SucursalDto dto);
        Task CambiarEstadoAsync(int id);
    }
}