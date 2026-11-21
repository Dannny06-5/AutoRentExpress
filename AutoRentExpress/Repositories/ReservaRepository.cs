using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly AppDbContext _context;

        public ReservaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reserva>> ObtenerTodosAsync()
        {
            return await _context.Reservas
                .AsNoTracking()
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(r => r.SucursalRetiro)
                .Include(r => r.SucursalDevolucion)
                .OrderByDescending(r => r.FechaRegistro)
                .ThenByDescending(r => r.Id)
                .ToListAsync();
        }

        public async Task<Reserva?> ObtenerPorIdAsync(int id)
        {
            return await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(r => r.SucursalRetiro)
                .Include(r => r.SucursalDevolucion)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Cliente?> ObtenerClienteAsync(int id)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Vehiculo?> ObtenerVehiculoAsync(int id)
        {
            return await _context.Vehiculos
                .Include(v => v.Modelo)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<EstadoVehiculo?> ObtenerEstadoVehiculoAsync(int id)
        {
            return await _context.EstadosVehiculo
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Sucursal?> ObtenerSucursalAsync(int id)
        {
            return await _context.Sucursales
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Cliente>> ObtenerClientesAsync()
        {
            return await _context.Clientes
                .AsNoTracking()
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombres)
                .ThenBy(c => c.Apellidos)
                .ToListAsync();
        }

        public async Task<List<Vehiculo>> ObtenerVehiculosAsync()
        {
            return await _context.Vehiculos
                .AsNoTracking()
                .Include(v => v.Modelo)
                .Where(v => v.Activo)
                .OrderBy(v => v.Placa)
                .ToListAsync();
        }

        public async Task<List<Sucursal>> ObtenerSucursalesAsync()
        {
            return await _context.Sucursales
                .AsNoTracking()
                .Where(s => s.Activa)
                .OrderBy(s => s.Nombre)
                .ToListAsync();
        }

        public async Task<bool> ExisteConflictoAsync(
            int vehiculoId,
            DateTime inicio,
            DateTime fin,
            int excluirReservaId = 0)
        {
            return await _context.Reservas.AnyAsync(r =>
                r.VehiculoId == vehiculoId &&
                r.Id != excluirReservaId &&
                r.Estado != "Cancelada" &&
                r.FechaInicio < fin &&
                inicio < r.FechaFin);
        }

        public async Task AgregarAsync(Reserva reserva)
        {
            await _context.Reservas.AddAsync(reserva);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}