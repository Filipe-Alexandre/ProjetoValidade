using ValiKop.Shared.Models.Enums;

namespace ValiKop.Shared.DTOs.Salgado
{
    public class SugestaoSalgadoDTO
    {
        public string Nome { get; set; } = string.Empty;
        public ValidadeHoras ValidadeHoras { get; set; }
        public ValidadeDias ValidadeDias { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
    }
}