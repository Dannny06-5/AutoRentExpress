using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IVehiculoRepository
    {
        Task<List<Vehiculo>> ObtenerTodosAsync();
        Task<Vehiculo?> ObtenerPorIdAsync(int id);
        Task<bool> ExistePlacaAsync(string placa, int excluirId = 0);
        Task<VehiculoCatalogosDatos> ObtenerCatalogosAsync();
        Task AgregarAsync(Vehiculo vehiculo);
        Task GuardarCambiosAsync();
    }

    public class VehiculoCatalogosDatos
    {
        public List<Modelo> Modelos { get; set; } = new();
        public List<CategoriaVehiculo> Categorias { get; set; } = new();
        public List<EstadoVehiculo> Estados { get; set; } = new();
        public List<Sucursal> Sucursales { get; set; } = new();
    }
}