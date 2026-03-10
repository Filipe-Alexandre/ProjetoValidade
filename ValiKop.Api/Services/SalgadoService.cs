using Microsoft.EntityFrameworkCore;
using ValiKop.Api.Data;
using ValiKop.Shared.DTOs.Salgado;
using ValiKop.Shared.Models;
using ValiKop.Shared.Models.Enums;
using ValiKop.Shared.Models.Mappings;

public class SalgadoService : ISalgadoService
{
    private readonly AppDbContext _context;

    public SalgadoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SalgadoDTO>> GetAllAsync()
    {
        return await _context.Salgados
            .AsNoTracking()
            .Where(s => s.Ativo)
            .Include(s => s.Categoria)
            .Select(s => new SalgadoDTO
            {
                Id = s.Id,
                Nome = s.Nome,
                Lote = s.Lote,
                ValidadeHoras = s.ValidadeHoras,
                ValidadeDias = s.ValidadeDias,
                Categoria = s.Categoria.Nome,
                Ativo = s.Ativo
            })
            .ToListAsync();
    }

    // --- BUSCAR APENAS INATIVOS ---
    public async Task<IEnumerable<SalgadoDTO>> GetInativosAsync()
    {
        return await _context.Salgados
            .AsNoTracking()
            .Where(s => !s.Ativo) // Filtra os inativos
            .Include(s => s.Categoria)
            .Select(s => new SalgadoDTO
            {
                Id = s.Id,
                Nome = s.Nome,
                Lote = s.Lote,
                ValidadeHoras = s.ValidadeHoras,
                ValidadeDias = s.ValidadeDias,
                Categoria = s.Categoria.Nome,
                Ativo = s.Ativo
            })
            .ToListAsync();
    }

    // --- SUGESTÕES DE AUTO-COMPLETE ---
    public async Task<IEnumerable<SugestaoSalgadoDTO>> GetSugestoesAsync()
    {
        return await _context.Salgados
            .Include(s => s.Categoria)
            .GroupBy(s => new
            {
                s.Nome,
                s.ValidadeHoras,
                s.ValidadeDias,
                s.CategoriaId,
                CategoriaNome = s.Categoria!.Nome
            })
            .Select(g => new SugestaoSalgadoDTO
            {
                Nome = g.Key.Nome,
                ValidadeHoras = g.Key.ValidadeHoras,
                ValidadeDias = g.Key.ValidadeDias,
                CategoriaId = g.Key.CategoriaId,
                CategoriaNome = g.Key.CategoriaNome
            })
            .OrderBy(s => s.Nome) // Retorna em ordem alfabética
            .ToListAsync();
    }

    public async Task<SalgadoDTO?> GetByIdAsync(int id)
    {
        var salgado = await _context.Salgados
            .Include(s => s.Categoria)
            .FirstOrDefaultAsync(s => s.Id == id && s.Ativo);

        if (salgado == null)
            return null;

        return new SalgadoDTO
        {
            Id = salgado.Id,
            Nome = salgado.Nome,
            Lote = salgado.Lote,
            ValidadeHoras = salgado.ValidadeHoras,
            ValidadeDias = salgado.ValidadeDias,
            Categoria = salgado.Categoria.Nome,
            Ativo = salgado.Ativo
        };
    }

    public async Task<SalgadoDTO> CreateAsync(SalgadoFormDTO dto, int usuarioId)
    {
        var salgado = new Salgado
        {
            Nome = dto.Nome,
            Lote = dto.Lote,
            ValidadeHoras = dto.ValidadeHoras,
            ValidadeDias = dto.ValidadeDias,
            CategoriaId = dto.CategoriaId,
            UsuarioId = usuarioId,
            Ativo = true
        };

        _context.Salgados.Add(salgado);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(salgado.Id)
            ?? throw new Exception("Erro ao criar salgado");
    }

    public async Task<SalgadoDTO> UpdateAsync(int id, SalgadoFormDTO dto)
    {
        var salgado = await _context.Salgados
            .FirstOrDefaultAsync(s => s.Id == id && s.Ativo);

        if (salgado == null)
            throw new Exception("Salgado não encontrado");

        salgado.Nome = dto.Nome;
        salgado.Lote = dto.Lote;
        salgado.ValidadeHoras = dto.ValidadeHoras;
        salgado.ValidadeDias = dto.ValidadeDias;
        salgado.CategoriaId = dto.CategoriaId;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id)
            ?? throw new Exception("Erro ao atualizar salgado");
    }

    // --- INATIVAR ---
    public async Task<SalgadoDTO> InativarAsync(int id)
    {
        var salgado = await _context.Salgados
            .Include(s => s.Categoria)
            .FirstOrDefaultAsync(s => s.Id == id && s.Ativo);

        if (salgado == null)
            throw new Exception("Salgado não encontrado ou já inativo");

        salgado.Ativo = false;
        await _context.SaveChangesAsync();

        return new SalgadoDTO
        {
            Id = salgado.Id,
            Nome = salgado.Nome,
            Lote = salgado.Lote,
            ValidadeHoras = salgado.ValidadeHoras,
            ValidadeDias = salgado.ValidadeDias,
            Categoria = salgado.Categoria.Nome,
            Ativo = salgado.Ativo
        };
    }

    // --- REATIVAR ---
    public async Task<SalgadoDTO> ReativarAsync(int id)
    {
        var salgado = await _context.Salgados
            .Include(s => s.Categoria)
            .FirstOrDefaultAsync(s => s.Id == id && !s.Ativo);

        if (salgado == null)
            throw new Exception("Salgado não encontrado ou já está ativo");

        salgado.Ativo = true;
        await _context.SaveChangesAsync();

        return new SalgadoDTO
        {
            Id = salgado.Id,
            Nome = salgado.Nome,
            Lote = salgado.Lote,
            ValidadeHoras = salgado.ValidadeHoras,
            ValidadeDias = salgado.ValidadeDias,
            Categoria = salgado.Categoria.Nome,
            Ativo = salgado.Ativo
        };
    }

    // --- HARD DELETE ---
    public async Task ExcluirDefinitivoAsync(int id)
    {
        var salgado = await _context.Salgados.FindAsync(id);

        if (salgado == null)
            throw new Exception("Salgado não encontrado");

        _context.Salgados.Remove(salgado);
        await _context.SaveChangesAsync();
    }

    public async Task<SalgadoPrintDTO?> GetPrintAsync(int id, int usuarioId)
    {
        var salgado = await _context.Salgados
            .Include(s => s.Usuario)
            .FirstOrDefaultAsync(s => s.Id == id && s.Ativo);

        if (salgado == null)
            return null;

        var retirada = DateTime.Now;
        var degelo = retirada.AddHours(
            ValidadeHorasMapper.ParaHoras(salgado.ValidadeHoras)
        );

        var validadeFinal = degelo.AddDays(
            ValidadeDiasMapper.ParaDias(salgado.ValidadeDias)
        );

        return new SalgadoPrintDTO
        {
            Nome = salgado.Nome,
            Lote = salgado.Lote,
            Retirada = retirada,
            Degelo = degelo,
            Validade = validadeFinal,
            Responsavel = salgado.Usuario.Nome
        };
    }
}