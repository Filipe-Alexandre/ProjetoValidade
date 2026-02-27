using ValiKop.Shared.DTOs.Salgado;

public interface ISalgadoService
{
    Task<IEnumerable<SalgadoDTO>> GetAllAsync();
    Task<SalgadoDTO?> GetByIdAsync(int id);
    Task<SalgadoDTO> CreateAsync(SalgadoFormDTO dto, int usuarioId);
    Task<SalgadoDTO> UpdateAsync(int id, SalgadoFormDTO dto);
    Task<SalgadoDTO> InativarAsync(int id);
    Task<SalgadoPrintDTO?> GetPrintAsync(int id, int usuarioId);
}
