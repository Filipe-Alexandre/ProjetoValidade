namespace ValiKop.Shared.DTOs.Produto
{
    public class ProdutoPrintDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Lote { get; set; } = string.Empty;

        public DateTime DataPrimaria { get; set; }
        public DateTime DataManuseio { get; set; }
        public DateTime DataSecundaria { get; set; }

        public string Responsavel { get; set; } = string.Empty;
    }
}
