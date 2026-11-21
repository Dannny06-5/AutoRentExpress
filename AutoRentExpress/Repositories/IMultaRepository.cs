using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IMultaRepository
    {
        Task<List<Multa>> ObtenerTodosAsync();

        Task<Multa?> ObtenerPorIdAsync(int id);

        Task<Contrato?> ObtenerContratoAsync(int id);

        Task<List<Contrato>> ObtenerContratosAsync();

        Task AgregarAsync(Multa multa);

        Task GuardarCambiosAsync();
    }
}