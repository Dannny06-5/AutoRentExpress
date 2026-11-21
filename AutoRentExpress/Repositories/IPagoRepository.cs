using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IPagoRepository
    {
        Task<List<Pago>> ObtenerTodosAsync();

        Task<Pago?> ObtenerPorIdAsync(int id);

        Task<Contrato?> ObtenerContratoAsync(int contratoId);

        Task<List<Contrato>> ObtenerContratosAsync();

        Task<decimal> ObtenerTotalPagadoAsync(int contratoId);

        Task<bool> ExisteReferenciaAsync(
            string referencia,
            string metodo);

        Task AgregarAsync(Pago pago);
    }
}