using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IModeloRepository
    {
        Task<List<Modelo>> ObtenerTodosAsync();
        Task<Modelo?> ObtenerPorIdAsync(int id);
        Task<bool> ExisteAsync(string nombre, int marcaId, int excluirId = 0);
        Task<bool> TieneVehiculosAsync(int id);
        Task AgregarAsync(Modelo modelo);
        void Eliminar(Modelo modelo);
        Task GuardarCambiosAsync();
    }
}