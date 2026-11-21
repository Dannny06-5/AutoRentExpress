
using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface ILicenciaConducirRepository
    {
        Task<List<LicenciaConducir>> ObtenerTodosAsync();

        Task<LicenciaConducir?> ObtenerPorIdAsync(int id);

        Task<Cliente?> ObtenerClienteAsync(int id);

        Task<List<Cliente>> ObtenerClientesAsync();

        Task<bool> ExisteNumeroAsync(string numero, int excluirId = 0);

        Task AgregarAsync(LicenciaConducir licencia);

        Task GuardarCambiosAsync();
    }
}
