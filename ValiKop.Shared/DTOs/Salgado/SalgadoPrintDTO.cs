namespace ValiKop.Shared.DTOs.Salgado
{
    public class SalgadoPrintDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Lote { get; set; } = string.Empty;

        public DateTime Retirada { get; set; } = DateTime.Now;
        public DateTime Degelo { get; set; }
        public DateTime Validade { get; set; }

        public string Responsavel { get; set; } = string.Empty;
    }
}
