using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class MantenimientoRepository : IMantenimientoRepository
    {
        private readonly AppDbContext _context;

        public MantenimientoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Mantenimiento>> ObtenerTodosAsync()
        {
            return await _context.Mantenimientos
                .AsNoTracking()
                .Include(m => m.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(m => m.Empleado)
                .OrderByDescending(m => m.FechaInicio)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<Mantenimiento?> ObtenerPorIdAsync(int id)
        {
            return await _context.Mantenimientos
                .Include(m => m.Vehiculo)
                    .ThenInclude(v => v.Modelo)
                .Include(m => m.Empleado)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Vehiculo?> ObtenerVehiculoAsync(int id)
        {
            return await _context.Vehiculos
                .Include(v => v.Modelo)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Empleado?> ObtenerEmpleadoAsync(int id)
        {
            return await _context.Empleados
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Vehiculo>> ObtenerVehiculosAsync()
        {
            return await _context.Vehiculos
                .AsNoTracking()
                .Include(v => v.Modelo)
                .OrderBy(v => v.Placa)
                .ToListAsync();
        }

        public async Task<List<Empleado>> ObtenerEmpleadosAsync()
        {
            return await _context.Empleados
                .AsNoTracking()
                .OrderBy(e => e.Nombres)
                .ThenBy(e => e.Apellidos)
                .ToListAsync();
        }

        public async Task<bool> ExisteMantenimientoActivoAsync(
            int vehiculoId,
            int excluirId = 0)
        {
            return await _context.Mantenimientos.AnyAsync(m =>
                m.VehiculoId == vehiculoId &&
                m.Id != excluirId &&
                m.Estado == "En proceso");
        }

        public async Task AgregarAsync(Mantenimiento mantenimiento)
        {
            await _context.Mantenimientos.AddAsync(mantenimiento);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}