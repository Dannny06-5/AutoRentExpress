using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class ContratoRepository : IContratoRepository
    {
        private readonly AppDbContext _context;

        public ContratoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Contrato>> ObtenerTodosAsync()
        {
            return await _context.Contratos
                .AsNoTracking()
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Cliente)
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .OrderByDescending(c => c.FechaFirma)
                .ThenByDescending(c => c.Id)
                .ToListAsync();
        }

        public async Task<Contrato?> ObtenerPorIdAsync(int id)
        {
            return await _context.Contratos
                .AsNoTracking()
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Cliente)
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Vehiculo)
                        .ThenInclude(v => v.Modelo)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Reserva?> ObtenerReservaAsync(int reservaId)
        {
            return await _context.Reservas
                .AsNoTracking()
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r => r.Id == reservaId);
        }

        public async Task<List<Reserva>> ObtenerReservasSinContratoAsync()
        {
            return await _context.Reservas
                .AsNoTracking()
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .Where(r =>
                    r.Estado == "Pendiente" &&
                    r.FechaInicio > DateTime.Now &&
                    !_context.Contratos.Any(c => c.ReservaId == r.Id))
                .OrderBy(r => r.FechaInicio)
                .ToListAsync();
        }

        public async Task<List<Empleado>> ObtenerEmpleadosAsync()
        {
            return await _context.Empleados
                .AsNoTracking()
                .Where(e => e.Activo)
                .OrderBy(e => e.Nombres)
                .ThenBy(e => e.Apellidos)
                .ToListAsync();
        }

        public async Task<bool> ExisteContratoReservaAsync(int reservaId)
        {
            return await _context.Contratos
                .AnyAsync(c => c.ReservaId == reservaId);
        }

        public async Task<bool> ExisteNumeroContratoAsync(
            string numeroContrato)
        {
            return await _context.Contratos
                .AnyAsync(c => c.NumeroContrato == numeroContrato);
        }

        public async Task AgregarAsync(Contrato contrato)
        {
            await _context.Contratos.AddAsync(contrato);
            await _context.SaveChangesAsync();
        }
    }
}