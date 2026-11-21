using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IInspeccionService
    {
        Task<List<InspeccionDto>> ObtenerTodosAsync();

        Task<InspeccionDto?> ObtenerPorIdAsync(int id);

        Task<InspeccionCatalogosDto> ObtenerCatalogosAsync();

        Task CrearAsync(InspeccionDto dto);

        Task ActualizarAsync(InspeccionDto dto);
    }
}