
using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class TarifaRepository : ITarifaRepository
    {
        private readonly AppDbContext _context;

        public TarifaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tarifa>> ObtenerTodosAsync()
        {
            return await _context.Tarifas
                .AsNoTracking()
                .Include(t => t.CategoriaVehiculo)
                .OrderByDescending(t => t.VigenteDesde)
                .ThenBy(t => t.Nombre)
                .ToListAsync();
        }

        public async Task<Tarifa?> ObtenerPorIdAsync(int id)
        {
            return await _context.Tarifas
                .Include(t => t.CategoriaVehiculo)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<CategoriaVehiculo?> ObtenerCategoriaAsync(int id)
        {
            return await _context.CategoriaVehiculo
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<CategoriaVehiculo>> ObtenerCategoriasAsync()
        {
            return await _context.CategoriaVehiculo
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExisteNombreAsync(
            string nombre,
            int categoriaId,
            int excluirId = 0)
        {
            var normalizado = nombre.Trim().ToUpper();

            return await _context.Tarifas.AnyAsync(t =>
                t.Id != excluirId &&
                t.CategoriaVehiculoId == categoriaId &&
                t.Nombre.ToUpper() == normalizado);
        }

        public async Task AgregarAsync(Tarifa tarifa)
        {
            await _context.Tarifas.AddAsync(tarifa);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
