using ValiKop.Shared.DTOs.Usuario;

namespace ValiKop.Shared.Interfaces
{
    public interface IUsuarioService
    {
        // ADMINISTRADOR
        // Cria usuário com senha temporária gerada
        Task<UsuarioDTO> AddAsync(UsuarioFormDTO dto, int adminId);

        // Atualiza dados do usuário (nome, tipo, ativo)
        Task<UsuarioDTO> UpdateAsync(int usuarioId, UsuarioFormDTO dto, int adminId);

        // Inativa usuário
        Task InativarAsync(int usuarioId, int adminId);

        // Reseta senha do usuário, gerando senha temporária
        Task<UsuarioResetSenhaResponseDTO> ResetarSenhaAsync(int usuarioId, int adminId);


        // USUÁRIO
        // Troca de senha (pode ser senha temporária ou não)
        Task AlterarSenhaAsync(int usuarioId, UsuarioAlterarSenhaDTO dto);


        // CONSULTAS
        // Lista todos os usuários ativos
        Task<List<UsuarioDTO>> GetAllAsync();

        // Busca usuário por ID
        Task<UsuarioDTO?> GetByIdAsync(int usuarioId);


        // LOGIN
        // Retorna usuário se login/senha estiverem corretos
        Task<UsuarioDTO?> LoginAsync(string login, string senha);
    }
}
