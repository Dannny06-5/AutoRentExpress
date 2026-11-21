using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IEstadoVehiculoService
    {
        Task<List<EstadoVehiculoDto>> ObtenerTodosAsync();
        Task<EstadoVehiculoDto?> ObtenerPorIdAsync(int id);
        Task CrearAsync(EstadoVehiculoDto dto);
        Task ActualizarAsync(EstadoVehiculoDto dto);
        Task EliminarAsync(int id);
    }
}