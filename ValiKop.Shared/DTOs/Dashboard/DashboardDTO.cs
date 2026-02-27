using ValiKop.Shared.Models.Enums;

namespace ValiKop.Shared.DTOs.Dashboard
{
    public class DashboardDTO
    {
        public int Id { get; set; }

        // Identificação do tipo (Produto ou Salgado)
        public string Tipo { get; set; } = string.Empty;

        // Dados comuns
        public string Nome { get; set; } = string.Empty;
        public string Lote { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        // Datas
        public DateTime DataPrimaria { get; set; }

        // Regras de validade (respeitando seu modelo)
        public int? ValidadeHoras { get; set; }
        public ValidadeDias? ValidadeDias { get; set; }

        public bool Ativo { get; set; }
    }
}
