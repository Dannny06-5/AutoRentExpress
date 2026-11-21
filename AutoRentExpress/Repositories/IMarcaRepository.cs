using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IMarcaRepository
    {
        Task<List<Marca>> ObtenerTodosAsync();
        Task<Marca?> ObtenerPorIdAsync(int id);
        Task<bool> ExisteNombreAsync(string nombre, int excluirId = 0);
        Task<bool> TieneModelosAsync(int id);
        Task AgregarAsync(Marca marca);
        void Eliminar(Marca marca);
        Task GuardarCambiosAsync();
    }
}