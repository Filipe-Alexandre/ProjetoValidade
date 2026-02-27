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

            return new CategoriaDTO
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Ativo = categoria.Ativo
            };
        }

        public async Task<List<CategoriaDTO>> GetAllAsync()
        {
            return await _context.Categorias
                .Where(c => c.Ativo)
                .Select(c => new CategoriaDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Ativo = c.Ativo
                })
                .ToListAsync();
        }

        public async Task<CategoriaDTO> GetByIdAsync(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id && c.Ativo);

            if (categoria == null)
                throw new NotFoundException("Categoria não encontrada.");

            return new CategoriaDTO
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Ativo = categoria.Ativo
            };
        }

        public async Task<CategoriaDTO> UpdateAsync(int id, CategoriaFormDTO dto)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id && c.Ativo);

            if (categoria == null)
                throw new NotFoundException("Categoria não encontrada.");

            categoria.Nome = dto.Nome;
            await _context.SaveChangesAsync();

            return new CategoriaDTO
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Ativo = categoria.Ativo
            };
        }

        public async Task<CategoriaDTO> InativarAsync(int id)
        {
            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(c => c.Id == id && c.Ativo);

            if (categoria == null)
                throw new NotFoundException("Categoria não encontrada.");

            categoria.Ativo = false;
            await _context.SaveChangesAsync();

            return new CategoriaDTO
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Ativo = categoria.Ativo
            };
        }
    }
}
