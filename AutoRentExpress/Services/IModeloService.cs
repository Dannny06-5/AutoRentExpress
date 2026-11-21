using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IModeloService
    {
        Task<List<ModeloDto>> ObtenerTodosAsync();
        Task<ModeloDto?> ObtenerPorIdAsync(int id);
        Task CrearAsync(ModeloDto dto);
        Task ActualizarAsync(ModeloDto dto);
        Task EliminarAsync(int id);
    }
}