using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class CategoriaVehiculoRepository : ICategoriaVehiculoRepository
    {
        private readonly AppDbContext _context;

        public CategoriaVehiculoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoriaVehiculo>> ObtenerTodosAsync()
        {
            return await _context.CategoriaVehiculo
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<CategoriaVehiculo?> ObtenerPorIdAsync(int id)
        {
            return await _context.CategoriaVehiculo
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int excluirId = 0)
        {
            return await _context.CategoriaVehiculo
                .AnyAsync(c => c.Nombre == nombre && c.Id != excluirId);
        }

        public async Task<bool> TieneRelacionesAsync(int id)
        {
            return await _context.Vehiculos
                .AnyAsync(v => v.CategoriaVehiculoId == id)
                || await _context.Tarifas
                .AnyAsync(t => t.CategoriaVehiculoId == id);
        }

        public async Task AgregarAsync(CategoriaVehiculo categoria)
        {
            await _context.CategoriaVehiculo.AddAsync(categoria);
        }

        public void Eliminar(CategoriaVehiculo categoria)
        {
            _context.CategoriaVehiculo.Remove(categoria);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}