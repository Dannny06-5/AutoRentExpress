using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IReservaService
    {
        Task<List<ReservaDto>> ObtenerTodosAsync();
        Task<ReservaDto?> ObtenerPorIdAsync(int id);
        Task<ReservaCatalogosDto> ObtenerCatalogosAsync();

        Task<List<ReservaOpcionDto>> ObtenerDisponiblesAsync(
            DateTime fechaInicio,
            DateTime fechaFin,
            int sucursalRetiroId,
            int reservaId = 0);

        Task CrearAsync(ReservaDto dto);
        Task ActualizarAsync(ReservaDto dto);
        Task CancelarAsync(int id);
    }
}