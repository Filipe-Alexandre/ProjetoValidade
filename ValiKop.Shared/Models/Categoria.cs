using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ValiKop.Shared.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [StringLength(20, ErrorMessage = "O Nome deve ter no máximo 20 caracteres.")]
        [DisplayName("Categoria")]
        public string Nome { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;
    }
}
