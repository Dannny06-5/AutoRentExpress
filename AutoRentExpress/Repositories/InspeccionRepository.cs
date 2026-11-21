using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class InspeccionRepository : IInspeccionRepository
    {
        private readonly AppDbContext _context;

        public InspeccionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Inspeccion>> ObtenerTodosAsync()
        {
            return await _context.Inspecciones
                .AsNoTracking()
                .Include(i => i.Contrato)
                    .ThenInclude(c => c.Reserva)
                        .ThenInclude(r => r.Cliente)
                .Include(i => i.Contrato)
                    .ThenInclude(c => c.Reserva)
                        .ThenInclude(r => r.Vehiculo)
                            .ThenInclude(v => v.Modelo)
                .Include(i => i.Empleado)
                .OrderByDescending(i => i.Fecha)
                .ThenByDescending(i => i.Id)
                .ToListAsync();
        }

        public async Task<Inspeccion?> ObtenerPorIdAsync(int id)
        {
            return await _context.Inspecciones
                .Include(i => i.Contrato)
                    .ThenInclude(c => c.Reserva)
                        .ThenInclude(r => r.Cliente)
                .Include(i => i.Contrato)
                    .ThenInclude(c => c.Reserva)
                        .ThenInclude(r => r.Vehiculo)
                            .ThenInclude(v => v.Modelo)
                .Include(i => i.Empleado)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Contrato?> ObtenerContratoAsync(int id)
        {
            return await _context.Contratos
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Vehiculo)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Empleado?> ObtenerEmpleadoAsync(int id)
        {
            return await _context.Empleados
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Contrato>> ObtenerContratosAsync()
        {
            return await _context.Contratos
                .AsNoTracking()
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Cliente)
                .Include(c => c.Reserva)
                    .ThenInclude(r => r.Vehiculo)
                .Where(c => c.Estado != "Anulado" &&
                            c.Estado != "Cancelado")
                .OrderByDescending(c => c.FechaFirma)
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

        public async Task<bool> ExisteTipoAsync(
            int contratoId,
            string tipo,
            int excluirId = 0)
        {
            return await _context.Inspecciones.AnyAsync(i =>
                i.ContratoId == contratoId &&
                i.Tipo == tipo &&
                i.Id != excluirId);
        }

        public async Task AgregarAsync(Inspeccion inspeccion)
        {
            await _context.Inspecciones.AddAsync(inspeccion);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}