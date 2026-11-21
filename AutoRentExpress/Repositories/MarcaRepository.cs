using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class MarcaRepository : IMarcaRepository
    {
        private readonly AppDbContext _context;

        public MarcaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Marca>> ObtenerTodosAsync()
        {
            return await _context.Marcas
                .AsNoTracking()
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<Marca?> ObtenerPorIdAsync(int id)
        {
            return await _context.Marcas
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> ExisteNombreAsync(string nombre, int excluirId = 0)
        {
            return await _context.Marcas
                .AnyAsync(m => m.Nombre == nombre && m.Id != excluirId);
        }

        public async Task<bool> TieneModelosAsync(int id)
        {
            return await _context.Modelos
                .AnyAsync(m => m.MarcaId == id);
        }

        public async Task AgregarAsync(Marca marca)
        {
            await _context.Marcas.AddAsync(marca);
        }

        public void Eliminar(Marca marca)
        {
            _context.Marcas.Remove(marca);
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}