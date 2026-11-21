using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface ICategoriaVehiculoService
    {
        Task<List<CategoriaVehiculoDto>> ObtenerTodosAsync();
        Task<CategoriaVehiculoDto?> ObtenerPorIdAsync(int id);
        Task CrearAsync(CategoriaVehiculoDto dto);
        Task ActualizarAsync(CategoriaVehiculoDto dto);
        Task EliminarAsync(int id);
    }
}