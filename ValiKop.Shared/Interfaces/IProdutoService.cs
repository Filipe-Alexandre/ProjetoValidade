using ValiKop.Shared.DTOs.Produto;

namespace ValiKop.Shared.Interfaces
{
    public interface IProdutoService
    {
        //CREATE
        Task<ProdutoDTO> AddAsync(ProdutoFormDTO dto, int usuarioId);

        //READ
        Task<List<ProdutoDTO>> GetAllAsync();
        Task<ProdutoFormDTO> GetByIdAsync(int id); 

        //UPDATE
        Task<ProdutoDTO> UpdateAsync(int id, ProdutoFormDTO produtoFormDTO);

        //DELETE (Soft)
        Task<ProdutoDTO> InativarAsync(int id);

        //GET BY CATEGORY
        Task<List<ProdutoDTO>> GetByCategoriaIdAsync(int categoriaId);

        //PRINT
        Task<ProdutoPrintDTO> GetParaImpressaoAsync(int produtoId, int usuarioId);
    }
}
