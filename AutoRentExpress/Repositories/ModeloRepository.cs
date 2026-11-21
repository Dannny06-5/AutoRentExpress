using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class ModeloRepository : IModeloRepository
    {
        private readonly AppDbContext _context;

        public ModeloRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Modelo>> ObtenerTodosAsync()
        {
            return await _context.Modelos
                .Include(m => m.Marca)
                .AsNoTracking()
                .OrderBy(m => m.Marca.Nombre)
                .ThenBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<Modelo?> ObtenerPorIdAsync(int id)
        {
            return await _context.Modelos
                .Include(m => m.Marca)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> ExisteAsync(
            string nombre, int marcaId, int excluirId = 0)
        {
            return await _context.Modelos
                .AnyAsync(m => m.Nombre == nombre &&
                               m.MarcaId == marcaId &&
                               m.Id != excluirId);
        }

        public async Task<bool> TieneVehiculosAsync(int id)
        {
            return await _context.Vehiculos
                .AnyAsync(v => v.ModeloId == id);
        }

        public async Task AgregarAsync(Modelo modelo)
        {
            await _context.Modelos.AddAsync(modelo);
        }

        public void Eliminar(Modelo modelo)
        {
            _context.Modelos.Remove(modelo);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}