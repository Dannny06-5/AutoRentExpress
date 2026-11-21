using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IClienteService
    {
        Task<List<ClienteDto>> ObtenerTodosAsync();
        Task<ClienteDto?> ObtenerPorIdAsync(int id);
        Task CrearAsync(ClienteDto dto);
        Task ActualizarAsync(ClienteDto dto);
        Task DesactivarAsync(int id);
        Task ActivarAsync(int id);
    }
}