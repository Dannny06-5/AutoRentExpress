using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class PagoRepository : IPagoRepository
    {
        private readonly AppDbContext _context;

        public PagoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pago>> ObtenerTodosAsync()
        {
            return await _context.Pagos
                .AsNoTracking()
                .Include(p => p.Contrato)
                    .ThenInclude(c => c.Reserva)
                        .ThenInclude(r => r.Cliente)
                .OrderByDescending(p => p.FechaPago)
                .ThenByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<Pago?> ObtenerPorIdAsync(int id)
        {
            return await _context.Pagos
                .AsNoTracking()
                .Include(p => p.Contrato)
                    .ThenInclude(c => c.Reserva)
                        .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Contrato?> ObtenerContratoAsync(int contratoId)
        {
            return await _context.Contratos
                .AsNoTracking()
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(c => c.Id == contratoId);
        }

        public async Task<List<Contrato>> ObtenerContratosAsync()
        {
            return await _context.Contratos
                .AsNoTracking()
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Cliente)
                .OrderByDescending(c => c.FechaFirma)
                .ToListAsync();
        }

        public async Task<decimal> ObtenerTotalPagadoAsync(int contratoId)
        {
            return await _context.Pagos
                .Where(p =>
                    p.ContratoId == contratoId &&
                    p.Estado == "Confirmado")
                .SumAsync(p => (decimal?)p.Monto) ?? 0m;
        }

        public async Task<bool> ExisteReferenciaAsync(
            string referencia,
            string metodo)
        {
            var referenciaNormalizada = referencia.Trim().ToUpper();
            var metodoNormalizado = metodo.Trim().ToUpper();

            return await _context.Pagos.AnyAsync(p =>
                p.Referencia != null &&
                p.Referencia.ToUpper() == referenciaNormalizada &&
                p.Metodo.ToUpper() == metodoNormalizado &&
                p.Estado == "Confirmado");
        }

        public async Task AgregarAsync(Pago pago)
        {
            await _context.Pagos.AddAsync(pago);
            await _context.SaveChangesAsync();
        }
    }
}