using ValiKop.Shared.DTOs.Salgado;

public interface ISalgadoService
{
    // READ
    Task<IEnumerable<SalgadoDTO>> GetAllAsync();
    Task<SalgadoDTO?> GetByIdAsync(int id);

    // READ DE INATIVOS
    Task<IEnumerable<SalgadoDTO>> GetInativosAsync();

    // --- SUGESTÕES DE AUTO-COMPLETE ---
    Task<IEnumerable<SugestaoSalgadoDTO>> GetSugestoesAsync();

    // CREATE / UPDATE
    Task<SalgadoDTO> CreateAsync(SalgadoFormDTO dto, int usuarioId);
    Task<SalgadoDTO> UpdateAsync(int id, SalgadoFormDTO dto);

    // SOFT DELETE (Inativar e Reativar)
    Task<SalgadoDTO> InativarAsync(int id);
    Task<SalgadoDTO> ReativarAsync(int id);

    // HARD DELETE (Excluir de vez)
    Task ExcluirDefinitivoAsync(int id);

    // PRINT
    Task<SalgadoPrintDTO?> GetPrintAsync(int id, int usuarioId);
}