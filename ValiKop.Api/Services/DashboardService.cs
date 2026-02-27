using Microsoft.EntityFrameworkCore;
using ValiKop.Api.Data;
using ValiKop.Shared.DTOs.Dashboard;
using ValiKop.Shared.Interfaces;
using ValiKop.Shared.Models.Mappings;

namespace ValiKop.Api.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        //INDEX
        public async Task<List<DashboardDTO>> GetAllAsync()
        {
            var produtos = await _context.Produtos
                .Where(p => p.Ativo)
                .Include(p => p.Categoria)
                .Select(p => new DashboardDTO
                {
                    Id = p.Id,
                    Tipo = "Produto",
                    Nome = p.Nome,
                    Lote = p.Lote,
                    Categoria = p.Categoria != null
                        ? p.Categoria.Nome
                        : "Sem categoria",
                    DataPrimaria = p.DataPrimaria,
                    ValidadeDias = p.ValidadeDias,
                    ValidadeHoras = null,
                    Ativo = p.Ativo
                })
                .ToListAsync();

            var salgados = await _context.Salgados
                .Where(s => s.Ativo)
                .Include(s => s.Categoria)
                .Select(s => new DashboardDTO
                {
                    Id = s.Id,
                    Tipo = "Salgado",
                    Nome = s.Nome,
                    Lote = s.Lote,
                    Categoria = s.Categoria != null
                        ? s.Categoria.Nome
                        : "Sem categoria",
                    DataPrimaria = DateTime.Now,
                    ValidadeHoras = (int)s.ValidadeHoras,
                    ValidadeDias = s.ValidadeDias,
                    Ativo = s.Ativo
                })
                .ToListAsync();

            return produtos
                .Concat(salgados)
                .OrderBy(x => x.Tipo)
                .ThenBy(x => x.Categoria)
                .ThenBy(x => x.Nome)
                .ThenBy(x => x.Id)
                .ToList();
        }

        //IMPRIMIR SELECIONADOS
        public async Task<List<DashboardPrintDTO>> PrintEmCascataAsync(
           DashboardPrintRequestDTO request,
           int usuarioId)
            {
            var usuario = await _context.Usuarios.FindAsync(usuarioId)
                ?? throw new Exception("Usuário não encontrado");

            var dataManuseio = DateTime.Now;

            // PRODUTOS
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => request.ProdutosIds.Contains(p.Id) && p.Ativo)
                .Select(p => new DashboardPrintDTO
                {
                    Nome = p.Nome,
                    Lote = p.Lote,
                    DataPrimaria = p.DataPrimaria,
                    DataManuseio = dataManuseio,
                    DataFinal = dataManuseio.AddDays(
                        ValidadeDiasMapper.ParaDias(p.ValidadeDias)
                    ),
                    Responsavel = usuario.Nome
                })
                .ToListAsync();

            // SALGADOS
            var salgados = await _context.Salgados
                .Include(s => s.Categoria)
                .Where(s => request.SalgadosIds.Contains(s.Id) && s.Ativo)
                .Select(s => new DashboardPrintDTO
                {
                    Nome = s.Nome,
                    Lote = s.Lote,
                    DataPrimaria = dataManuseio, // retirada = agora
                    DataManuseio = dataManuseio.AddHours(
                        ValidadeHorasMapper.ParaHoras(s.ValidadeHoras)
                    ),
                    DataFinal = dataManuseio
                        .AddHours(ValidadeHorasMapper.ParaHoras(s.ValidadeHoras))
                        .AddDays(ValidadeDiasMapper.ParaDias(s.ValidadeDias)),
                    Responsavel = usuario.Nome
                })
                .ToListAsync();

            return produtos
                .Concat(salgados)
                .OrderBy(x => x.Tipo)
                .ThenBy(x => x.Categoria)
                .ThenBy(x => x.Nome)
                .ThenBy(x => x.Id)
                .ToList();
        }
    }
}
