using ValiKop.Shared.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ValiKop.Shared.DTOs.Salgado
{
    public class SalgadoFormDTO
    {
        [Required]
        [DisplayName("Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [DisplayName("Lote")]
        public string Lote { get; set; } = string.Empty;

        [Required]
        [DisplayName("Horas de Degelo")]
        public ValidadeHoras ValidadeHoras { get; set; }

        [Required]
        [DisplayName("Dias de Validade")]
        public ValidadeDias ValidadeDias { get; set; }

        [Required]
        [DisplayName("Categoria")]
        public int CategoriaId { get; set; }
    }
}
