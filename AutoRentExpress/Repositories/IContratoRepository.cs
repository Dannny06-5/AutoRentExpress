
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
        Task<bool> ExisteOtroContratoEnCursoAsync(int vehiculoId, int excluirContratoId);
        Task<bool> ExisteInspeccionAsync(int contratoId, string tipo);
        Task<EstadoVehiculo?> ObtenerEstadoVehiculoAsync(string nombre);
        Task AgregarAsync(Contrato contrato);
        Task GuardarCambiosAsync();
    }
}
