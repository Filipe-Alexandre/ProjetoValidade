using ValiKop.Shared.DTOs.Usuario;

namespace ValiKop.Shared.Interfaces
{
    public interface IUsuarioService
    {
        // ADMINISTRADOR
        Task<UsuarioDTO> AddAsync(UsuarioFormDTO dto, int adminId);
        Task<UsuarioDTO> UpdateAsync(int usuarioId, UsuarioFormDTO dto, int adminId);
        Task<UsuarioResetSenhaResponseDTO> ResetarSenhaAsync(int usuarioId, int adminId);

        // --- PADRÃO DE DELEÇÃO E LIXEIRA ---
        Task<UsuarioDTO> InativarAsync(int usuarioId, int adminId);
        Task<UsuarioDTO> ReativarAsync(int usuarioId, int adminId);
        Task ExcluirDefinitivoAsync(int usuarioId, int adminId);

        // CONSULTAS
        Task<List<UsuarioDTO>> GetAllAsync();
        Task<UsuarioDTO?> GetByIdAsync(int usuarioId);
        Task<List<UsuarioDTO>> GetInativosAsync(); // NOVO

        // USUÁRIO
        Task AlterarSenhaAsync(int usuarioId, UsuarioAlterarSenhaDTO dto);
    }
}