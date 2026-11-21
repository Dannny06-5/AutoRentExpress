using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class EstadoVehiculoRepository : IEstadoVehiculoRepository
    {
        private readonly AppDbContext _context;

        public EstadoVehiculoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EstadoVehiculo>> ObtenerTodosAsync()
        {
            return await _context.EstadosVehiculo
                .AsNoTracking()
                .OrderBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<EstadoVehiculo?> ObtenerPorIdAsync(int id)
        {
            return await _context.EstadosVehiculo
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> ExisteNombreAsync(
            string nombre, int excluirId = 0)
        {
            return await _context.EstadosVehiculo
                .AnyAsync(e => e.Nombre == nombre && e.Id != excluirId);
        }

        public async Task<bool> TieneVehiculosAsync(int id)
        {
            return await _context.Vehiculos
                .AnyAsync(v => v.EstadoVehiculoId == id);
        }

        public async Task AgregarAsync(EstadoVehiculo estado)
        {
            await _context.EstadosVehiculo.AddAsync(estado);
        }

        public void Eliminar(EstadoVehiculo estado)
        {
            _context.EstadosVehiculo.Remove(estado);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}