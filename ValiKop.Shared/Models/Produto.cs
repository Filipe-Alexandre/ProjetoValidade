using ValiKop.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ValiKop.Shared.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        // Dados essenciais
        [Required]
        [StringLength(30)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string Lote { get; set; } = string.Empty;

        [Required]
        public DateTime DataPrimaria { get; set; }

        // Regra de validade (contagem controlada)
        [Required]
        public ValidadeDias ValidadeDias { get; set; }

        public bool Ativo { get; set; } = true;

        // Relacionamentos
        [Required]
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;

        [Required]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
    }
}
