using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly AppDbContext _context;

        public EmpleadoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Empleado>> ObtenerTodosAsync()
        {
            return await _context.Empleados
                .AsNoTracking()
                .Include(e => e.Sucursal)
                .OrderBy(e => e.Nombres)
                .ThenBy(e => e.Apellidos)
                .ToListAsync();
        }

        public async Task<Empleado?> ObtenerPorIdAsync(int id)
        {
            return await _context.Empleados
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Sucursal?> ObtenerSucursalAsync(int id)
        {
            return await _context.Sucursales
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Sucursal>> ObtenerSucursalesAsync()
        {
            return await _context.Sucursales
                .AsNoTracking()
                .OrderBy(s => s.Nombre)
                .ToListAsync();
        }

        public async Task AgregarAsync(Empleado empleado)
        {
            await _context.Empleados.AddAsync(empleado);
            await _context.SaveChangesAsync();
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}