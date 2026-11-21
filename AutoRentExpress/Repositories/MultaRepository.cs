using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class MultaRepository : IMultaRepository
    {
        private readonly AppDbContext _context;

        public MultaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Multa>> ObtenerTodosAsync()
        {
            return await _context.Multas
                .AsNoTracking()
                .Include(m => m.Contrato)
                    .ThenInclude(c => c.Reserva)
                        .ThenInclude(r => r.Cliente)
                .OrderByDescending(m => m.Fecha)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<Multa?> ObtenerPorIdAsync(int id)
        {
            return await _context.Multas
                .Include(m => m.Contrato)
                    .ThenInclude(c => c.Reserva)
                        .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Contrato?> ObtenerContratoAsync(int id)
        {
            return await _context.Contratos
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Contrato>> ObtenerContratosAsync()
        {
            return await _context.Contratos
                .AsNoTracking()
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Cliente)
                .Where(c => c.Estado != "Anulado" &&
                            c.Estado != "Cancelado")
                .OrderByDescending(c => c.FechaFirma)
                .ToListAsync();
        }

        public async Task AgregarAsync(Multa multa)
        {
            await _context.Multas.AddAsync(multa);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}