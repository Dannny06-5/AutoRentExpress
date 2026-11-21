using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface ISeguroRepository
    {
        Task<List<Seguro>> ObtenerTodosAsync();

        Task<Seguro?> ObtenerPorIdAsync(int id);

        Task<bool> ExisteNombreAsync(string nombre, int excluirId = 0);

        Task AgregarAsync(Seguro seguro);

        Task GuardarCambiosAsync();
    }
}