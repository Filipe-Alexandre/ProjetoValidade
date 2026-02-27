namespace ValiKop.Shared.DTOs.Usuario
{
    public class UsuarioAlterarSenhaDTO
    {
        public string SenhaAtual { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
        public string ConfirmacaoSenha { get; set; } = string.Empty;
    }
}
