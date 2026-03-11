using ValiKop.Shared.DTOs.Produto;

namespace ValiKop.Shared.Interfaces
{
    public interface IProdutoService
    {
        // CREATE
        Task<ProdutoDTO> AddAsync(ProdutoFormDTO dto, int usuarioId);

        // READ
        Task<List<ProdutoDTO>> GetAllAsync();
        Task<ProdutoFormDTO?> GetByIdAsync(int id);

        // READ DE INATIVOS
        Task<List<ProdutoDTO>> GetInativosAsync();

        // SUGESTÕES DE AUTO-COMPLETE
        Task<List<SugestaoProdutoDTO>> GetSugestoesAsync();

        // UPDATE
        Task<ProdutoDTO> UpdateAsync(int id, ProdutoFormDTO produtoFormDTO);

        // SOFT DELETE (Inativar e Reativar)
        Task<ProdutoDTO> InativarAsync(int id);
        Task<ProdutoDTO> ReativarAsync(int id);

        // HARD DELETE (Excluir de vez)
        Task ExcluirDefinitivoAsync(int id);

        // GET BY CATEGORY
        Task<List<ProdutoDTO>> GetByCategoriaIdAsync(int categoriaId);

        // PRINT
        Task<ProdutoPrintDTO> GetParaImpressaoAsync(int produtoId, int usuarioId);
    }
}