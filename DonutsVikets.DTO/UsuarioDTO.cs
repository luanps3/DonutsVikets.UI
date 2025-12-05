// DonutsVikets.DTOs/UsuarioDTO.cs
namespace DonutsVikets.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Senha será enviada pela UI apenas quando criar/alterar.
        // Não persista a senha em texto.
        public string Senha { get; set; } = string.Empty;

        public int TipoUsuarioId { get; set; }
        public string TipoUsuarioNome { get; set; } = string.Empty;
    }
}
