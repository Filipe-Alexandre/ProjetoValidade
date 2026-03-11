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

        public async Task<UsuarioDTO> UpdateAsync(int usuarioId, UsuarioFormDTO dto, int adminId)
        {
            var admin = await _context.Usuarios.FindAsync(adminId);
            if (admin == null || !admin.Ativo || admin.TipoUsuario != TipoUsuario.Administrador)
                throw new Exception("Administrador inválido.");

            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
                throw new Exception("Usuário não encontrado.");

            var loginEmUso = await _context.Usuarios
        .AnyAsync(u => u.Login == dto.Login && u.Id != usuarioId);

            usuario.Nome = dto.Nome;
            usuario.Login = dto.Login;
            usuario.TipoUsuario = dto.TipoUsuario;
            usuario.Ativo = dto.Ativo;

            await _context.SaveChangesAsync();

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                User = usuario.Login,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                Ativo = usuario.Ativo,
                PasswordTemp = usuario.PasswordTemp
            };
        }

        // --- INATIVAR ---
        public async Task<UsuarioDTO> InativarAsync(int usuarioId, int adminId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId)
                ?? throw new Exception("Usuário não encontrado.");

            // Impede que o próprio admin se inative por acidente (opcional, mas recomendado)
            if (usuarioId == adminId)
                throw new Exception("Você não pode inativar a si mesmo.");

            usuario.Ativo = false;
            await _context.SaveChangesAsync();

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                Ativo = usuario.Ativo,
                PasswordTemp = usuario.PasswordTemp
            };
        }

        // --- REATIVAR ---
        public async Task<UsuarioDTO> ReativarAsync(int usuarioId, int adminId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId)
                ?? throw new Exception("Usuário não encontrado.");

            usuario.Ativo = true;
            await _context.SaveChangesAsync();

            return new UsuarioDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                TipoUsuario = usuario.TipoUsuario.ToString(),
                Ativo = usuario.Ativo,
                PasswordTemp = usuario.PasswordTemp
            };
        }

        // --- EXCLUIR DEFINITIVO (HARD DELETE) ---
        public async Task ExcluirDefinitivoAsync(int usuarioId, int adminId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId)
                ?? throw new Exception("Usuário não encontrado.");

            if (usuarioId == adminId)
                throw new Exception("Você não pode excluir a si mesmo.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
        }

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
                Login = usuario.Login,
                SenhaTemporaria = novaSenha
            };
        }

        public async Task AlterarSenhaAsync(int usuarioId, UsuarioAlterarSenhaDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId)
                ?? throw new Exception("Usuário não encontrado.");

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

        // --- FILTRANDO ATIVOS ---
        public async Task<List<UsuarioDTO>> GetAllAsync()
        {
            return await _context.Usuarios
                .Where(u => u.Ativo) // Filtra apenas os ativos
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

        // --- BUSCAR INATIVOS ---
        public async Task<List<UsuarioDTO>> GetInativosAsync()
        {
            return await _context.Usuarios
                .Where(u => !u.Ativo)
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

        public async Task<UsuarioDTO?> GetByIdAsync(int usuarioId)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId && u.Ativo);
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
    }
}