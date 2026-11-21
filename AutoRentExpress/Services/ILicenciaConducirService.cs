
using AutoRentExpress.DTOs;

namespace AutoRentExpress.Services
{
    public interface ILicenciaConducirService
    {
        Task<List<LicenciaConducirDto>> ObtenerTodosAsync();

        Task<LicenciaConducirDto?> ObtenerPorIdAsync(int id);

        Task<List<LicenciaClienteDto>> ObtenerClientesAsync();

        Task CrearAsync(LicenciaConducirDto dto);

        Task ActualizarAsync(LicenciaConducirDto dto);
    }
}
