namespace ValiKop.Shared.DTOs.Dashboard
{
    // O pacote completo que o Front-end vai receber de uma vez só
    public class DashboardResumoDTO
    {
        public int TotalProdutosAtivos { get; set; }
        public int TotalSalgadosAtivos { get; set; }
        public List<AlertaVencimentoDTO> AlertasVencimento { get; set; } = new();
        public List<TopCadastroDTO> TopCadastros { get; set; } = new();
        public List<GraficoCategoriaDTO> GraficoCategorias { get; set; } = new();
    }

    // Regra 1: Alerta de 7 dias
    public class AlertaVencimentoDTO
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = string.Empty; // Produto
        public string Nome { get; set; } = string.Empty;
        public string Lote { get; set; } = string.Empty;
        public DateTime DataPrimaria { get; set; }
    }

    // Regra 2: Itens com mais lotes
    public class TopCadastroDTO
    {
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; // Produto ou Salgado
        public int QuantidadeLotes { get; set; }
    }

    // Regra 3: Gráfico de Categorias
    public class GraficoCategoriaDTO
    {
        public string Categoria { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }
}