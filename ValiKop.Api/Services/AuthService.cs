using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ValiKop.Api.Data;
using ValiKop.Api.Security;
using ValiKop.Shared.DTOs.Auth;
using ValiKop.Shared.DTOs.Usuario;
using ValiKop.Shared.Interfaces;
using ValiKop.Shared.Models.Enums;

namespace ValiKop.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task AlterarSenhaAsync(int usuarioId, UsuarioAlterarSenhaDTO dto)
        {
            if (dto.NovaSenha != dto.ConfirmacaoSenha)
                throw new Exception("A nova senha e a confirmação não conferem.");

            // Adicionada trava de segurança: Usuário deve existir E estar Ativo
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId && u.Ativo)
                ?? throw new Exception("Acesso negado: Usuário inexistente ou inativo.");

            if (!usuario.PasswordTemp)
            {
                var senhaAtualValida = PasswordHasher.Verify(
                    dto.SenhaAtual!,
                    usuario.PasswordHash,
                    usuario.PasswordSalt
                );

                if (!senhaAtualValida)
                    throw new Exception("Senha atual inválida.");
            }

            var (hash, salt) = PasswordHasher.Hash(dto.NovaSenha);

            usuario.PasswordHash = hash;
            usuario.PasswordSalt = salt;
            usuario.PasswordTemp = false;

            await _context.SaveChangesAsync();
        }

        public async Task<LoginResultDTO?> LoginAsync(LoginDTO dto)
        {
            // Busca apenas usuários ATIVOS. Se estiver inativo, retorna nulo imediatamente.
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Login == dto.User && u.Ativo);

            if (usuario == null)
                return null;

            var senhaValida = PasswordHasher.Verify(
                dto.Password,
                usuario.PasswordHash,
                usuario.PasswordSalt
            );

            if (!senhaValida)
                return null;

            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                 new Claim(ClaimTypes.Name, usuario.Nome),
                 new Claim(ClaimTypes.Role, usuario.TipoUsuario.ToString()),
                 new Claim("PasswordTemp", usuario.PasswordTemp.ToString().ToLower())
            };

            string key = _configuration["Jwt:Key"];
            string issuer = _configuration["Jwt:Issuer"];
            string audience = _configuration["Jwt:Audience"];
            int expireHoras = int.Parse(_configuration["Jwt:ExpireHoras"]);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expireHoras),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResultDTO
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                TipoUsuario = usuario.TipoUsuario,
                ForcarTrocaSenha = usuario.PasswordTemp,
                Token = tokenString
            };
        }

        public Task LogoutAsync()
        {
            return Task.CompletedTask;
        }
    }
}