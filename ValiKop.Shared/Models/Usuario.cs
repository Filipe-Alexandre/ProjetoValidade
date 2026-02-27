using ValiKop.Shared.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ValiKop.Shared.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(15, ErrorMessage = "O campo Nome deve ter no máximo 15 caracteres.")]
        [DisplayName("Nome")]
        public string Nome { get; set; } = string.Empty;

        public TipoUsuario TipoUsuario { get; set; } = TipoUsuario.Comum;

        public bool Ativo { get; set; } = true;

        // Credenciais de Acesso
        [Required(ErrorMessage = "O campo Usuário é obrigatório.")]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Usuário deve ter entre 6 e 15 caracteres.")]
        [DisplayName("Usuário")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "O campo Senha é obrigatório.")]
        [DisplayName("Senha")]
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;

        public bool PasswordTemp { get; set; }
    }
}
