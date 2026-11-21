using AutoRentExpress.Models;

namespace AutoRentExpress.Repositories
{
    public interface IInspeccionRepository
    {
        Task<List<Inspeccion>> ObtenerTodosAsync();

        Task<Inspeccion?> ObtenerPorIdAsync(int id);

        Task<Contrato?> ObtenerContratoAsync(int id);

        Task<Empleado?> ObtenerEmpleadoAsync(int id);

        Task<List<Contrato>> ObtenerContratosAsync();

        Task<List<Empleado>> ObtenerEmpleadosAsync();

        Task<bool> ExisteTipoAsync(
            int contratoId,
            string tipo,
            int excluirId = 0);

        Task AgregarAsync(Inspeccion inspeccion);

        Task GuardarCambiosAsync();
    }
}