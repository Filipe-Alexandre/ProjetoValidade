using ValiKop.Shared.Models.Enums;

namespace ValiKop.Shared.DTOs.Usuario
{
    public class UsuarioFormDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public TipoUsuario TipoUsuario { get; set; }
        public bool Ativo { get; set; } = true;
    }

}
