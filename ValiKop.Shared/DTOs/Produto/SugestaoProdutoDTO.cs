using ValiKop.Shared.Models.Enums;

namespace ValiKop.Shared.DTOs.Produto
{
    public class SugestaoProdutoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public ValidadeDias ValidadeDias { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
    }
}