using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface ICategoriaVehiculoRepository
    {
        Task<List<CategoriaVehiculo>> ObtenerTodosAsync();
        Task<CategoriaVehiculo?> ObtenerPorIdAsync(int id);
        Task<bool> ExisteNombreAsync(string nombre, int excluirId = 0);
        Task<bool> TieneRelacionesAsync(int id);
        Task AgregarAsync(CategoriaVehiculo categoria);
        void Eliminar(CategoriaVehiculo categoria);
        Task GuardarCambiosAsync();
    }
}