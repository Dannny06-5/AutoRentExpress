using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IMantenimientoService
    {
        Task<List<MantenimientoDto>> ObtenerTodosAsync();

        Task<MantenimientoDto?> ObtenerPorIdAsync(int id);

        Task<MantenimientoCatalogosDto> ObtenerCatalogosAsync();

        Task CrearAsync(MantenimientoDto dto);

        Task ActualizarAsync(MantenimientoDto dto);

        Task IniciarAsync(int id);

        Task FinalizarAsync(int id);
    }
}