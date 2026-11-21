using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IReservaRepository
    {
        Task<List<Reserva>> ObtenerTodosAsync();
        Task<Reserva?> ObtenerPorIdAsync(int id);
        Task<Cliente?> ObtenerClienteAsync(int id);
        Task<Vehiculo?> ObtenerVehiculoAsync(int id);
        Task<EstadoVehiculo?> ObtenerEstadoVehiculoAsync(int id);
        Task<Sucursal?> ObtenerSucursalAsync(int id);
        Task<List<Cliente>> ObtenerClientesAsync();
        Task<List<Vehiculo>> ObtenerVehiculosAsync();
        Task<List<Sucursal>> ObtenerSucursalesAsync();

        Task<bool> ExisteConflictoAsync(
            int vehiculoId,
            DateTime inicio,
            DateTime fin,
            int excluirReservaId = 0);

        Task AgregarAsync(Reserva reserva);
        Task GuardarCambiosAsync();
    }
}