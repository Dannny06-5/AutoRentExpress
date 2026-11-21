
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
                .Include(c => c.Empleado)
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
                .Include(c => c.Empleado)
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

        public async Task<bool> ExisteNumeroContratoAsync(string numeroContrato)
        {
            return await _context.Contratos
                .AnyAsync(c => c.NumeroContrato == numeroContrato);
        }

        public async Task<bool> ExisteOtroContratoEnCursoAsync(
            int vehiculoId,
            int excluirContratoId)
        {
            return await _context.Contratos.AnyAsync(c =>
                c.Id != excluirContratoId &&
                c.Estado == "En curso" &&
                c.Reserva.VehiculoId == vehiculoId);
        }

        public async Task<bool> ExisteInspeccionAsync(
            int contratoId,
            string tipo)
        {
            return await _context.Inspecciones.AnyAsync(i =>
                i.ContratoId == contratoId &&
                i.Tipo == tipo);
        }

        public async Task<EstadoVehiculo?> ObtenerEstadoVehiculoAsync(
            string nombre)
        {
            var estados = await _context.EstadosVehiculo
                .AsNoTracking()
                .ToListAsync();

            return estados.FirstOrDefault(e =>
                Normalizar(e.Nombre) == Normalizar(nombre));
        }

        public async Task AgregarAsync(Contrato contrato)
        {
            await _context.Contratos.AddAsync(contrato);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }

        private static string Normalizar(string valor)
        {
            return valor.Trim().TrimEnd('.').Trim().ToUpperInvariant();
        }
    }
}
