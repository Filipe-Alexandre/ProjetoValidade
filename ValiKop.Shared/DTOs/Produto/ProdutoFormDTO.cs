using ValiKop.Shared.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ValiKop.Shared.DTOs.Produto
{
    public class ProdutoFormDTO
    {
        [Required]
        [StringLength(30)]
        [DisplayName("Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        [DisplayName("Lote")]
        public string Lote { get; set; } = string.Empty;

        [Required]
        [DisplayName("Data Primária")]
        public DateTime DataPrimaria { get; set; }

        [Required]
        [DisplayName("Validade")]
        public ValidadeDias ValidadeDias { get; set; }

        [Required]
        [DisplayName("Categoria")]
        public int CategoriaId { get; set; }
    }
}
