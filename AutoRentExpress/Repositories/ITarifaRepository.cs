
using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface ITarifaRepository
    {
        Task<List<Tarifa>> ObtenerTodosAsync();

        Task<Tarifa?> ObtenerPorIdAsync(int id);

        Task<CategoriaVehiculo?> ObtenerCategoriaAsync(int id);

        Task<List<CategoriaVehiculo>> ObtenerCategoriasAsync();

        Task<bool> ExisteNombreAsync(
            string nombre,
            int categoriaId,
            int excluirId = 0);

        Task AgregarAsync(Tarifa tarifa);

        Task GuardarCambiosAsync();
    }
}
