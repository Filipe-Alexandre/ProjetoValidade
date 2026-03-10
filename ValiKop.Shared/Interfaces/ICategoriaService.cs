using ValiKop.Shared.DTOs.Categoria;

namespace ValiKop.Shared.Interfaces
{
    public interface ICategoriaService
    {
        // CREATE
        Task<CategoriaDTO> AddAsync(CategoriaFormDTO categoriaFormDTO);

        // READ
        Task<List<CategoriaDTO>> GetAllAsync();
        Task<CategoriaDTO> GetByIdAsync(int id);

        // READ DE INATIVOS
        Task<List<CategoriaDTO>> GetInativosAsync();

        // UPDATE
        Task<CategoriaDTO> UpdateAsync(int id, CategoriaFormDTO categoriaFormDTO);

        // SOFT DELETE (Inativar e Reativar)
        Task<CategoriaDTO> InativarAsync(int id);
        Task<CategoriaDTO> ReativarAsync(int id);

        // HARD DELETE (Excluir de vez)
        Task DeleteAsync(int id);
    }
}