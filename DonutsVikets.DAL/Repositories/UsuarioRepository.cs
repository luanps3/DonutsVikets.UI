// Project.DAL/Repositories/UsuarioRepository.cs
using DonutsVikets.Models;
using Microsoft.EntityFrameworkCore;
using Project.DAL.Data;


namespace DonutsVikets.DAL.Repositories
{
    // Repositório concreto. Você não usa interfaces — então aqui está direto.
    public class UsuarioRepository
    {
        private readonly DonutsVikets3Context _context;

        public UsuarioRepository(DonutsVikets3Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .Include(u => u.TipoUsuario)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ent = await _context.Usuarios.FindAsync(id);
            if (ent != null)
            {
                _context.Usuarios.Remove(ent);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == id);
        }
    }
}
