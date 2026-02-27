using ValiKop.Shared.Models.Enums;

namespace ValiKop.Shared.DTOs.Usuario
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string TipoUsuario { get; set; }
        public bool Ativo { get; set; }
        public bool PasswordTemp { get; set; }
        public string SenhaTemp { get; set; }
    }
}
