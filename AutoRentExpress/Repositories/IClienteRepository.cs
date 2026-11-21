using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> ObtenerTodosAsync();
        Task<Cliente?> ObtenerPorIdAsync(int id);
        Task<bool> ExisteIdentificacionAsync(string identificacion, int excluirId = 0);
        Task AgregarAsync(Cliente cliente);
        Task ActualizarAsync(Cliente cliente);
        Task GuardarCambiosAsync();
    }
}