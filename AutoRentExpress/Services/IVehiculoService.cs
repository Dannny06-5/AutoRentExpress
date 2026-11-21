using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IVehiculoService
    {
        Task<List<VehiculoDto>> ObtenerTodosAsync();
        Task<VehiculoDto?> ObtenerPorIdAsync(int id);
        Task<VehiculoCatalogosDto> ObtenerCatalogosAsync();
        Task CrearAsync(VehiculoDto dto);
        Task ActualizarAsync(VehiculoDto dto);
        Task CambiarEstadoActivoAsync(int id);
    }
}