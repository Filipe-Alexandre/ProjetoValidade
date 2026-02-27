using Microsoft.EntityFrameworkCore;
using ValiKop.Api.Data;
using ValiKop.Shared.DTOs.Usuario;
using ValiKop.Shared.Interfaces;
using ValiKop.Shared.Models;
using ValiKop.Shared.Models.Enums;
using ValiKop.Api.Security;

namespace ValiKop.Api.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;

        public UsuarioService(AppDbContext context)
        {
            _context = context;
        }

        // ADMIN: criar usuário
        public async Task<UsuarioDTO> AddAsync(UsuarioFormDTO dto, int adminId)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Login == dto.Login))
                throw new Exception("Login já existente.");

            var senhaTemp = PasswordGenerator.Generate();
            var (hash, salt) = PasswordHasher.Hash(senhaTemp);

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Login = dto.Login,
                TipoUsuario = dto.TipoUsuario,
                PasswordHash = hash,
                PasswordSalt = salt,
                PasswordTemp = true,
                Ativo = true
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                Ativo = usuario.Ativo,
                PasswordTemp = usuario.PasswordTemp,
                SenhaTemp = senhaTemp
            };
        }

        // ADMIN: atualizar usuário
        public async Task<UsuarioDTO> UpdateAsync(int usuarioId, UsuarioFormDTO dto, int adminId)
        {
            // verificar se o admin existe e é ativo
            var admin = await _context.Usuarios.FindAsync(adminId);
            if (admin == null || !admin.Ativo || admin.TipoUsuario != TipoUsuario.Administrador)
                throw new Exception("Administrador inválido.");

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            // atualizar dados básicos
            usuario.Nome = dto.Nome;
            usuario.TipoUsuario = dto.TipoUsuario;
            usuario.Ativo = dto.Ativo;


            // retorna DTO atualizado
            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                Ativo = usuario.Ativo,
                PasswordTemp = usuario.PasswordTemp
            };
            await _context.SaveChangesAsync();
        }

        // ADMIN: inativar usuário
        public async Task InativarAsync(int usuarioId, int adminId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId)
                ?? throw new Exception("Usuário não encontrado.");

            usuario.Ativo = false;
            await _context.SaveChangesAsync();
        }

        // ADMIN: resetar senha
        public async Task<UsuarioResetSenhaResponseDTO> ResetarSenhaAsync(int usuarioId, int adminId)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId)
                ?? throw new Exception("Usuário não encontrado.");

            var novaSenha = PasswordGenerator.Generate();
            var (hash, salt) = PasswordHasher.Hash(novaSenha);

            usuario.PasswordHash = hash;
            usuario.PasswordSalt = salt;
            usuario.PasswordTemp = true;

            await _context.SaveChangesAsync();

            return new UsuarioResetSenhaResponseDTO
            {
                Login = usuario.Login,           // 👈 login real
                SenhaTemporaria = novaSenha      // 👈 senha em texto
            };
        }



        // USUÁRIO: alterar senha
        public async Task AlterarSenhaAsync(int usuarioId, UsuarioAlterarSenhaDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId)
                ?? throw new Exception("Usuário não encontrado.");

            // se não for senha temporária, valida senha atual
            if (!usuario.PasswordTemp)
            {
                var senhaAtualValida = PasswordHasher.Verify(
                    dto.SenhaAtual,
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

        
        // GET ALL
        public async Task<List<UsuarioDTO>> GetAllAsync()
        {
            return await _context.Usuarios
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nome = u.Nome,
                    TipoUsuario = u.TipoUsuario.ToString(),
                    Ativo = u.Ativo,
                    PasswordTemp = u.PasswordTemp
                })
                .ToListAsync();
        }

        // GET BY ID
        public async Task<UsuarioDTO?> GetByIdAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null) return null;

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                Ativo = usuario.Ativo,
                PasswordTemp = usuario.PasswordTemp
            };
        }
        
        // LOGIN
        public async Task<UsuarioDTO?> LoginAsync(string login, string senha)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Login == login && u.Ativo);

            if (usuario == null) return null;

            var senhaValida = PasswordHasher.Verify(senha, usuario.PasswordHash, usuario.PasswordSalt);
            if (!senhaValida) return null;

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                Ativo = usuario.Ativo,
                PasswordTemp = usuario.PasswordTemp
            };
        }

    }
}
