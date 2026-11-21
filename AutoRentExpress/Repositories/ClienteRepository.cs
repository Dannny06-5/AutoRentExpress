using AutoRentExpress.Data;
using AutoRentExpress.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoRentExpress.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly AppDbContext _context;

        public ClienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            return await _context.Clientes
                .AsNoTracking()
                .OrderBy(c => c.Nombres)
                .ThenBy(c => c.Apellidos)
                .ToListAsync();
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            return await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExisteIdentificacionAsync(
            string identificacion, int excluirId = 0)
        {
            return await _context.Clientes
                .AnyAsync(c => c.Identificacion == identificacion &&
                               c.Id != excluirId);
        }

        public async Task AgregarAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
        }

        public Task ActualizarAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            return Task.CompletedTask;
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}