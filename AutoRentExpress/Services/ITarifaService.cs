
using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface ITarifaService
    {
        Task<List<TarifaDto>> ObtenerTodosAsync();

        Task<TarifaDto?> ObtenerPorIdAsync(int id);

        Task<List<TarifaCategoriaDto>> ObtenerCategoriasAsync();

        Task CrearAsync(TarifaDto dto);

        Task ActualizarAsync(TarifaDto dto);

        Task CambiarEstadoAsync(int id);
    }
}
