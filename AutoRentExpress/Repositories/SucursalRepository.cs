using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class SucursalRepository : ISucursalRepository
    {
        private readonly AppDbContext _context;

        public SucursalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sucursal>> ObtenerTodosAsync()
        {
            return await _context.Sucursales
                .AsNoTracking()
                .OrderBy(s => s.Nombre)
                .ToListAsync();
        }

        public async Task<Sucursal?> ObtenerPorIdAsync(int id)
        {
            return await _context.Sucursales
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> ExisteNombreAsync(
            string nombre, int excluirId = 0)
        {
            return await _context.Sucursales
                .AnyAsync(s => s.Nombre == nombre && s.Id != excluirId);
        }

        public async Task AgregarAsync(Sucursal sucursal)
        {
            await _context.Sucursales.AddAsync(sucursal);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}