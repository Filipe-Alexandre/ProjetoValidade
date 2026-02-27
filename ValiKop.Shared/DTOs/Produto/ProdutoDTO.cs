using ValiKop.Shared.Models.Enums;

namespace ValiKop.Shared.DTOs.Produto
{
    public class ProdutoDTO
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;
        public string Lote { get; set; } = string.Empty;

        public DateTime DataPrimaria { get; set; }
        public ValidadeDias ValidadeDias { get; set; }

        public string Categoria { get; set; } = string.Empty;

        public bool Ativo { get; set; }

    }
}
