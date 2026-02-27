public class DashboardPrintDTO
{
    public string Tipo { get; set; } = string.Empty; // Produto | Salgado

    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    public string Lote { get; set; } = string.Empty;

    public DateTime DataPrimaria { get; set; }
    public DateTime DataManuseio { get; set; }
    public DateTime DataFinal { get; set; }

    public string Categoria { get; set; } = string.Empty;
    public string Responsavel { get; set; } = string.Empty;
}
