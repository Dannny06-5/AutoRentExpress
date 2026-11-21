using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IMarcaService
    {
        Task<List<MarcaDto>> ObtenerTodosAsync();
        Task<MarcaDto?> ObtenerPorIdAsync(int id);
        Task CrearAsync(MarcaDto dto);
        Task ActualizarAsync(MarcaDto dto);
        Task EliminarAsync(int id);
    }
}