using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface ISucursalRepository
    {
        Task<List<Sucursal>> ObtenerTodosAsync();
        Task<Sucursal?> ObtenerPorIdAsync(int id);
        Task<bool> ExisteNombreAsync(string nombre, int excluirId = 0);
        Task AgregarAsync(Sucursal sucursal);
        Task GuardarCambiosAsync();
    }
}