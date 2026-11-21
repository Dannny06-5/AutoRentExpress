using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface ISeguroService
    {
        Task<List<SeguroDto>> ObtenerTodosAsync();

        Task<SeguroDto?> ObtenerPorIdAsync(int id);

        Task CrearAsync(SeguroDto dto);

        Task ActualizarAsync(SeguroDto dto);

        Task CambiarEstadoActivoAsync(int id);
    }
}