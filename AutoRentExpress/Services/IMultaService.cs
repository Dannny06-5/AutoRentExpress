using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IMultaService
    {
        Task<List<MultaDto>> ObtenerTodosAsync();

        Task<MultaDto?> ObtenerPorIdAsync(int id);

        Task<List<MultaContratoDto>> ObtenerContratosAsync();

        Task CrearAsync(MultaDto dto);

        Task ActualizarAsync(MultaDto dto);

        Task MarcarPagadaAsync(int id);
    }
}