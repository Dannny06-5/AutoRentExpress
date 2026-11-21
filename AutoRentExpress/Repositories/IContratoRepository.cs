using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IContratoRepository
    {
        Task<List<Contrato>> ObtenerTodosAsync();

        Task<Contrato?> ObtenerPorIdAsync(int id);

        Task<Reserva?> ObtenerReservaAsync(int reservaId);

        Task<List<Reserva>> ObtenerReservasSinContratoAsync();

        Task<List<Empleado>> ObtenerEmpleadosAsync();

        Task<bool> ExisteContratoReservaAsync(int reservaId);

        Task<bool> ExisteNumeroContratoAsync(string numeroContrato);

        Task AgregarAsync(Contrato contrato);
    }
}