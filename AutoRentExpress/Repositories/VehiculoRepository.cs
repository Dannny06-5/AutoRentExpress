using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class VehiculoRepository : IVehiculoRepository
    {
        private readonly AppDbContext _context;

        public VehiculoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vehiculo>> ObtenerTodosAsync()
        {
            return await _context.Vehiculos
                .Include(v => v.Modelo)
                    .ThenInclude(m => m.Marca)
                .Include(v => v.CategoriaVehiculo)
                .Include(v => v.EstadoVehiculo)
                .Include(v => v.Sucursal)
                .AsNoTracking()
                .OrderBy(v => v.Placa)
                .ToListAsync();
        }

        public async Task<Vehiculo?> ObtenerPorIdAsync(int id)
        {
            return await _context.Vehiculos
                .Include(v => v.Modelo)
                    .ThenInclude(m => m.Marca)
                .Include(v => v.CategoriaVehiculo)
                .Include(v => v.EstadoVehiculo)
                .Include(v => v.Sucursal)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<bool> ExistePlacaAsync(string placa, int excluirId = 0)
        {
            return await _context.Vehiculos
                .AnyAsync(v => v.Placa == placa && v.Id != excluirId);
        }

        public async Task<VehiculoCatalogosDatos> ObtenerCatalogosAsync()
        {
            return new VehiculoCatalogosDatos
            {
                Modelos = await _context.Modelos
                    .Include(m => m.Marca)
                    .AsNoTracking()
                    .OrderBy(m => m.Marca.Nombre)
                    .ThenBy(m => m.Nombre)
                    .ToListAsync(),

                Categorias = await _context.CategoriaVehiculo
                    .AsNoTracking()
                    .OrderBy(c => c.Nombre)
                    .ToListAsync(),

                Estados = await _context.EstadosVehiculo
                    .AsNoTracking()
                    .OrderBy(e => e.Nombre)
                    .ToListAsync(),

                Sucursales = await _context.Sucursales
                    .AsNoTracking()
                    .Where(s => s.Activa)
                    .OrderBy(s => s.Nombre)
                    .ToListAsync()
            };
        }

        public async Task AgregarAsync(Vehiculo vehiculo)
        {
            await _context.Vehiculos.AddAsync(vehiculo);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}