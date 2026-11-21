using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IContratoService
    {
        Task<List<ContratoDto>> ObtenerTodosAsync();

        Task<ContratoDto?> ObtenerPorIdAsync(int id);

        Task<ContratoCatalogosDto> ObtenerCatalogosAsync();

        Task CrearAsync(ContratoDto dto);
    }
}