using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class SeguroRepository : ISeguroRepository
    {
        private readonly AppDbContext _context;

        public SeguroRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Seguro>> ObtenerTodosAsync()
        {
            return await _context.Seguros
                .AsNoTracking()
                .OrderBy(s => s.Nombre)
                .ToListAsync();
        }

        public async Task<Seguro?> ObtenerPorIdAsync(int id)
        {
            return await _context.Seguros
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> ExisteNombreAsync(
            string nombre,
            int excluirId = 0)
        {
            var nombreNormalizado = nombre.Trim().ToUpper();

            return await _context.Seguros.AnyAsync(s =>
                s.Id != excluirId &&
                s.Nombre.ToUpper() == nombreNormalizado);
        }

        public async Task AgregarAsync(Seguro seguro)
        {
            await _context.Seguros.AddAsync(seguro);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}