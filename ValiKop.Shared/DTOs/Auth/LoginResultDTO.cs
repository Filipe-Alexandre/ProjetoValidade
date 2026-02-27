using ValiKop.Shared.Models.Enums;

namespace ValiKop.Shared.DTOs.Auth
{
    public class LoginResultDTO
    {
        public int UsuarioId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public TipoUsuario TipoUsuario { get; set; }
        public bool ForcarTrocaSenha { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}