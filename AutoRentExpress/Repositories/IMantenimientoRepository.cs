using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IMantenimientoRepository
    {
        Task<List<Mantenimiento>> ObtenerTodosAsync();

        Task<Mantenimiento?> ObtenerPorIdAsync(int id);

        Task<Vehiculo?> ObtenerVehiculoAsync(int id);

        Task<Empleado?> ObtenerEmpleadoAsync(int id);

        Task<List<Vehiculo>> ObtenerVehiculosAsync();

        Task<List<Empleado>> ObtenerEmpleadosAsync();

        Task<bool> ExisteMantenimientoActivoAsync(
            int vehiculoId,
            int excluirId = 0);

        Task AgregarAsync(Mantenimiento mantenimiento);

        Task GuardarCambiosAsync();
    }
}