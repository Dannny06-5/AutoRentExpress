using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IEmpleadoRepository
    {
        Task<List<Empleado>> ObtenerTodosAsync();

        Task<Empleado?> ObtenerPorIdAsync(int id);

        Task<Sucursal?> ObtenerSucursalAsync(int id);

        Task<List<Sucursal>> ObtenerSucursalesAsync();

        Task AgregarAsync(Empleado empleado);

        Task GuardarCambiosAsync();
    }
}