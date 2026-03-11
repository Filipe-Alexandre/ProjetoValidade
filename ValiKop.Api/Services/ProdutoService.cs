using Microsoft.EntityFrameworkCore;
using ValiKop.Api.Data;
using ValiKop.Shared.DTOs.Produto;
using ValiKop.Api.Exceptions;
using ValiKop.Shared.Interfaces;
using ValiKop.Shared.Models;
using ValiKop.Shared.Models.Mappings;

namespace ValiKop.Api.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly AppDbContext _context;

        public ProdutoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProdutoDTO> AddAsync(ProdutoFormDTO dto, int usuarioId)
        {
            var existe = await _context.Produtos.AnyAsync(p =>
                p.Nome == dto.Nome &&
                p.Lote == dto.Lote &&
                p.Ativo);

            if (existe)
                throw new BusinessException("Já existe um produto ativo com este nome e lote.");

            var produto = new Produto
            {
                Nome = dto.Nome,
                Lote = dto.Lote,
                DataPrimaria = dto.DataPrimaria,
                ValidadeDias = dto.ValidadeDias,
                CategoriaId = dto.CategoriaId,
                UsuarioId = usuarioId,
                Ativo = true
            };

            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();

            return new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Lote = produto.Lote,
                DataPrimaria = produto.DataPrimaria,
                Categoria = produto.Categoria!.Nome,
                Ativo = true
            };
        }

        public async Task<List<ProdutoDTO>> GetAllAsync()
        {
            return await _context.Produtos
                .Include(p => p.Categoria) // Garantir que a Categoria seja carregada
                .Where(p => p.Ativo)
                .Select(p => new ProdutoDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Lote = p.Lote,
                    DataPrimaria = p.DataPrimaria,
                    ValidadeDias = p.ValidadeDias,
                    Categoria = p.Categoria!.Nome,
                    Ativo = p.Ativo
                })
                .ToListAsync();
        }

        // --- GET INATIVOS ---
        public async Task<List<ProdutoDTO>> GetInativosAsync()
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => !p.Ativo) // Busca apenas os que estão com Ativo = false
                .Select(p => new ProdutoDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Lote = p.Lote,
                    DataPrimaria = p.DataPrimaria,
                    ValidadeDias = p.ValidadeDias,
                    Categoria = p.Categoria!.Nome,
                    Ativo = p.Ativo
                })
                .ToListAsync();
        }

        // --- SUGESTÕES DE AUTO-COMPLETE ---
        public async Task<List<SugestaoProdutoDTO>> GetSugestoesAsync()
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .GroupBy(p => new
                {
                    p.Nome,
                    p.ValidadeDias,
                    p.CategoriaId,
                    CategoriaNome = p.Categoria!.Nome
                })
                .Select(g => new SugestaoProdutoDTO
                {
                    Nome = g.Key.Nome,
                    ValidadeDias = g.Key.ValidadeDias,
                    CategoriaId = g.Key.CategoriaId,
                    CategoriaNome = g.Key.CategoriaNome
                })
                .OrderBy(s => s.Nome) // Retorna em ordem alfabética
                .ToListAsync();
        }

        public async Task<ProdutoFormDTO?> GetByIdAsync(int id)
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);

            if (produto == null)
                return null;

            return new ProdutoFormDTO
            {
                Nome = produto.Nome,
                Lote = produto.Lote,
                DataPrimaria = produto.DataPrimaria,
                ValidadeDias = produto.ValidadeDias,
                CategoriaId = produto.CategoriaId
            };
        }

        public async Task<ProdutoDTO> UpdateAsync(int id, ProdutoFormDTO dto)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
                throw new NotFoundException("Produto não encontrado.");

            if (!produto.Ativo)
                throw new BusinessException("Não é possível editar um produto inativo.");

            var duplicado = await _context.Produtos.AnyAsync(p =>
                p.Id != id &&
                p.Nome == dto.Nome &&
                p.Lote == dto.Lote &&
                p.Ativo);

            if (duplicado)
                throw new BusinessException("Já existe outro produto ativo com este nome e lote.");

            produto.Nome = dto.Nome;
            produto.Lote = dto.Lote;
            produto.DataPrimaria = dto.DataPrimaria;
            produto.ValidadeDias = dto.ValidadeDias;
            produto.CategoriaId = dto.CategoriaId;

            await _context.SaveChangesAsync();
            await _context.Entry(produto).Reference(p => p.Categoria).LoadAsync();

            return new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Lote = produto.Lote,
                DataPrimaria = produto.DataPrimaria,
                ValidadeDias = produto.ValidadeDias,
                Categoria = produto.Categoria?.Nome,
                Ativo = produto.Ativo
            };
        }

        // --- INATIVAR ---
        public async Task<ProdutoDTO> InativarAsync(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);

            if (produto == null)
                throw new NotFoundException("Produto não encontrado ou já está inativo.");

            produto.Ativo = false;
            await _context.SaveChangesAsync();

            return new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Lote = produto.Lote,
                DataPrimaria = produto.DataPrimaria,
                ValidadeDias = produto.ValidadeDias,
                Categoria = produto.Categoria?.Nome,
                Ativo = produto.Ativo
            };
        }

        // --- REATIVAR ---
        public async Task<ProdutoDTO> ReativarAsync(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id && !p.Ativo);

            if (produto == null)
                throw new NotFoundException("Produto não encontrado ou já está ativo.");

            var duplicado = await _context.Produtos.AnyAsync(p =>
                p.Nome == produto.Nome &&
                p.Lote == produto.Lote &&
                p.Ativo);

            if (duplicado)
                throw new BusinessException("Não é possível reativar este produto, pois já existe outro ativo com o mesmo nome e lote.");

            produto.Ativo = true;
            await _context.SaveChangesAsync();

            return new ProdutoDTO
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Lote = produto.Lote,
                DataPrimaria = produto.DataPrimaria,
                ValidadeDias = produto.ValidadeDias,
                Categoria = produto.Categoria?.Nome,
                Ativo = produto.Ativo
            };
        }

        // --- HARD DELETE ---
        public async Task ExcluirDefinitivoAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
                throw new NotFoundException("Produto não encontrado.");

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ProdutoDTO>> GetByCategoriaIdAsync(int categoriaId)
        {
            return await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Ativo && p.CategoriaId == categoriaId)
                .Select(p => new ProdutoDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Lote = p.Lote,
                    DataPrimaria = p.DataPrimaria,
                    Categoria = p.Categoria!.Nome,
                    Ativo = p.Ativo
                })
                .ToListAsync();
        }

        public async Task<ProdutoPrintDTO> GetParaImpressaoAsync(int produtoId, int usuarioId)
        {
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == produtoId && p.Ativo);

            if (produto == null)
                throw new NotFoundException("Produto não encontrado ou inativo.");

            var usuario = await _context.Usuarios.FindAsync(usuarioId)
                ?? throw new NotFoundException("Usuário não encontrado.");

            var dataManuseio = DateTime.Now;
            var dataSecundaria = dataManuseio.AddDays(ValidadeDiasMapper.ParaDias(produto.ValidadeDias));

            return new ProdutoPrintDTO
            {
                Nome = produto.Nome,
                Lote = produto.Lote,
                DataPrimaria = produto.DataPrimaria,
                DataManuseio = dataManuseio,
                DataSecundaria = dataSecundaria,
                Responsavel = usuario.Nome
            };
        }
    }
}