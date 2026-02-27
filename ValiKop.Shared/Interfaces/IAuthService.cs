using ValiKop.Shared.DTOs.Auth;
using ValiKop.Shared.DTOs.Usuario;

namespace ValiKop.Shared.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResultDTO> LoginAsync(LoginDTO dto);
        Task LogoutAsync();
        Task AlterarSenhaAsync(int usuarioId, UsuarioAlterarSenhaDTO dto);
    }
}
