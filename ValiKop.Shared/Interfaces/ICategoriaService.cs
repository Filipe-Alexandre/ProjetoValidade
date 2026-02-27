using ValiKop.Shared.DTOs.Categoria;

namespace ValiKop.Shared.Interfaces
{
    public interface ICategoriaService
    {
        //CREATE
        Task<CategoriaDTO> AddAsync(CategoriaFormDTO categoriaFormDTO);

        //READ
        Task<List<CategoriaDTO>> GetAllAsync();
        Task<CategoriaDTO> GetByIdAsync(int id);

        //UPDATE
        Task<CategoriaDTO> UpdateAsync(int id, CategoriaFormDTO categoriaFormDTO);

        //DELETE (Soft)
        Task<CategoriaDTO> InativarAsync(int id);
    }
}
