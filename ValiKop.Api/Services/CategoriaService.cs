using Microsoft.EntityFrameworkCore;
using ValiKop.Api.Data;
using ValiKop.Shared.DTOs.Categoria;
using ValiKop.Api.Exceptions;
using ValiKop.Shared.Interfaces;
using ValiKop.Shared.Models;

namespace ValiKop.Api.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly AppDbContext _context;

        public CategoriaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoriaDTO> AddAsync(CategoriaFormDTO dto)
        {
            var categoria = new Categoria
            {
                Nome = dto.Nome,
                Ativo = true
            };

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return new CategoriaDTO { Id = categoria.Id, Nome = categoria.Nome, Ativo = categoria.Ativo };
        }

        public async Task<List<CategoriaDTO>> GetAllAsync()
        {
            // O GetAll padrão continua trazendo apenas os ativos
            return await _context.Categorias
                .Where(c => c.Ativo)
                .Select(c => new CategoriaDTO { Id = c.Id, Nome = c.Nome, Ativo = c.Ativo })
                .ToListAsync();
        }

        // --- BUSCAR APENAS INATIVOS ---
        public async Task<List<CategoriaDTO>> GetInativosAsync()
        {
            return await _context.Categorias
                .Where(c => !c.Ativo) // Filtra onde Ativo == false
                .Select(c => new CategoriaDTO { Id = c.Id, Nome = c.Nome, Ativo = c.Ativo })
                .ToListAsync();
        }

        public async Task<CategoriaDTO> GetByIdAsync(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id && c.Ativo);

            if (categoria == null)
                throw new NotFoundException("Categoria não encontrada.");

            return new CategoriaDTO { Id = categoria.Id, Nome = categoria.Nome, Ativo = categoria.Ativo };
        }

        public async Task<CategoriaDTO> UpdateAsync(int id, CategoriaFormDTO dto)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id && c.Ativo);

            if (categoria == null)
                throw new NotFoundException("Categoria não encontrada.");

            categoria.Nome = dto.Nome;
            await _context.SaveChangesAsync();

            return new CategoriaDTO { Id = categoria.Id, Nome = categoria.Nome, Ativo = categoria.Ativo };
        }

        // --- INATIVAR ---
        public async Task<CategoriaDTO> InativarAsync(int id)
        {
            // Busca apenas se estiver ativo
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id && c.Ativo);

            if (categoria == null)
                throw new NotFoundException("Categoria não encontrada ou já inativa.");

            categoria.Ativo = false;
            await _context.SaveChangesAsync();

            return new CategoriaDTO { Id = categoria.Id, Nome = categoria.Nome, Ativo = categoria.Ativo };
        }

        // --- REATIVAR ---
        public async Task<CategoriaDTO> ReativarAsync(int id)
        {
            // Busca apenas se estiver inativo
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id && !c.Ativo);

            if (categoria == null)
                throw new NotFoundException("Categoria não encontrada ou já está ativa.");

            categoria.Ativo = true;
            await _context.SaveChangesAsync();

            return new CategoriaDTO { Id = categoria.Id, Nome = categoria.Nome, Ativo = categoria.Ativo };
        }

        // --- HARD DELETE ---
        public async Task DeleteAsync(int id)
        {
            // Usa FindAsync pois não importa se está ativo ou não, queremos apagar
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
                throw new NotFoundException("Categoria não encontrada.");

            _context.Categorias.Remove(categoria); // Apaga de verdade do SQL
            await _context.SaveChangesAsync();
        }
    }
}