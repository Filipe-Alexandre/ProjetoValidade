using ValiKop.Shared.Models.Enums;

namespace ValiKop.Shared.DTOs.Salgado
{
    public class SalgadoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Lote { get; set; } = string.Empty;

        public ValidadeHoras ValidadeHoras { get; set; }
        public ValidadeDias ValidadeDias { get; set; }

        public string Categoria { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
