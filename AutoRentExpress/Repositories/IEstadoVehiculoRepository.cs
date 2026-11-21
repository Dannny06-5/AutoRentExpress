using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IEstadoVehiculoRepository
    {
        Task<List<EstadoVehiculo>> ObtenerTodosAsync();
        Task<EstadoVehiculo?> ObtenerPorIdAsync(int id);
        Task<bool> ExisteNombreAsync(string nombre, int excluirId = 0);
        Task<bool> TieneVehiculosAsync(int id);
        Task AgregarAsync(EstadoVehiculo estado);
        void Eliminar(EstadoVehiculo estado);
        Task GuardarCambiosAsync();
    }
}