// Project.BLL/Services/UsuarioService.cs
using DonutsVikets.DAL.Repositories;
using DonutsVikets.DTOs;
using DonutsVikets.Models;
using System.Security.Cryptography;
using System.Text;

namespace Project.BLL.Services
{
    // Serviço de negócio que usa UsuarioRepository diretamente (sem interface).
    public class UsuarioService
    {
        private readonly UsuarioRepository _repo;

        public UsuarioService(UsuarioRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<UsuarioDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            var dto = new List<UsuarioDTO>();
            foreach (var u in list)
            {
                dto.Add(EntityToDto(u));
            }
            return dto;
        }

        public async Task<UsuarioDTO?> GetByIdAsync(int id)
        {
            var u = await _repo.GetByIdAsync(id);
            return u == null ? null : EntityToDto(u);
        }

        public async Task<(bool success, string message)> CreateAsync(UsuarioDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome)) return (false, "Nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(dto.Email)) return (false, "Email é obrigatório.");
            if (string.IsNullOrWhiteSpace(dto.Senha)) return (false, "Senha é obrigatória.");

            var existing = await _repo.GetByEmailAsync(dto.Email);
            if (existing != null) return (false, "Já existe usuário com esse e-mail.");

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = HashPassword(dto.Senha),
                TipoUsuarioId = dto.TipoUsuarioId
            };

            await _repo.AddAsync(usuario);
            return (true, "Criado com sucesso.");
        }

        public async Task<(bool success, string message)> UpdateAsync(UsuarioDTO dto)
        {
            if (!await _repo.ExistsAsync(dto.Id)) return (false, "Usuário não encontrado.");
            var u = await _repo.GetByIdAsync(dto.Id);
            if (u == null) return (false, "Usuário não encontrado.");

            u.Nome = dto.Nome;
            u.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Senha))
            {
                u.SenhaHash = HashPassword(dto.Senha);
            }
            u.TipoUsuarioId = dto.TipoUsuarioId;

            await _repo.UpdateAsync(u);
            return (true, "Atualizado com sucesso.");
        }

        public async Task<(bool success, string message)> DeleteAsync(int id)
        {
            if (!await _repo.ExistsAsync(id)) return (false, "Usuário não existe.");
            await _repo.DeleteAsync(id);
            return (true, "Removido com sucesso.");
        }

        private UsuarioDTO EntityToDto(Usuario u)
        {
            return new UsuarioDTO
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email,
                TipoUsuarioId = u.TipoUsuarioId,
                TipoUsuarioNome = u.TipoUsuario?.Nome ?? string.Empty
            };
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder();
            foreach (var b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
