using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface IPagoService
    {
        Task<List<PagoDto>> ObtenerTodosAsync();

        Task<PagoDto?> ObtenerPorIdAsync(int id);

        Task<List<PagoContratoDto>> ObtenerContratosAsync();

        Task RegistrarAsync(PagoDto dto);
    }
}