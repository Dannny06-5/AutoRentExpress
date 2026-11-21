
using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class LicenciaConducirRepository : ILicenciaConducirRepository
    {
        private readonly AppDbContext _context;

        public LicenciaConducirRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LicenciaConducir>> ObtenerTodosAsync()
        {
            return await _context.Set<LicenciaConducir>()
                .AsNoTracking()
                .Include(l => l.Cliente)
                .OrderBy(l => l.FechaVencimiento)
                .ThenBy(l => l.Numero)
                .ToListAsync();
        }

        public async Task<LicenciaConducir?> ObtenerPorIdAsync(int id)
        {
            return await _context.Set<LicenciaConducir>()
                .Include(l => l.Cliente)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Cliente?> ObtenerClienteAsync(int id)
        {
            return await _context.Set<Cliente>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Cliente>> ObtenerClientesAsync()
        {
            return await _context.Set<Cliente>()
                .AsNoTracking()
                .OrderBy(c => c.Nombres)
                .ThenBy(c => c.Apellidos)
                .ToListAsync();
        }

        public async Task<bool> ExisteNumeroAsync(
            string numero,
            int excluirId = 0)
        {
            var normalizado = numero.Trim().ToUpper();

            return await _context.Set<LicenciaConducir>()
                .AnyAsync(l =>
                    l.Id != excluirId &&
                    l.Numero.ToUpper() == normalizado);
        }

        public async Task AgregarAsync(LicenciaConducir licencia)
        {
            await _context.Set<LicenciaConducir>().AddAsync(licencia);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
