using ValiKop.Shared.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ValiKop.Shared.Models
{
    public class Salgado
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Lote { get; set; } = string.Empty;

        [Required]
        public ValidadeHoras ValidadeHoras { get; set; } // 2h, 12h, etc

        [Required]
        public ValidadeDias ValidadeDias { get; set; } // 1,2,3,5,7

        public bool Ativo { get; set; } = true;

        // FK Usuário
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        // FK Categoria
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;
    }
}
