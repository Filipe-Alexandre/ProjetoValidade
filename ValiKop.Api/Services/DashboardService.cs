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

        // --- RESUMO DO DASHBOARD (MÉTRICAS) ---
        public async Task<DashboardResumoDTO> GetResumoAsync()
        {
            var hoje = DateTime.Now.Date;
            var limiteVencimento = hoje.AddDays(7);

            // 1. Contadores Simples
            var totalProdutos = await _context.Produtos.CountAsync(p => p.Ativo);
            var totalSalgados = await _context.Salgados.CountAsync(s => s.Ativo);

            // 2. Alertas (Apenas Produtos, baseando-se na Data Primária nos próximos 7 dias)
            var alertas = await _context.Produtos
                .Where(p => p.Ativo && p.DataPrimaria.Date >= hoje && p.DataPrimaria.Date <= limiteVencimento)
                .Select(p => new AlertaVencimentoDTO
                {
                    Id = p.Id,
                    Tipo = "Produto",
                    Nome = p.Nome,
                    Lote = p.Lote,
                    DataPrimaria = p.DataPrimaria
                })
                .OrderBy(a => a.DataPrimaria)
                .ToListAsync();

            // 3. Top Cadastros (Agrupa pelo Nome e conta os Lotes)
            var topProdutos = await _context.Produtos
                .Where(p => p.Ativo)
                .GroupBy(p => p.Nome)
                .Select(g => new TopCadastroDTO { Nome = g.Key, Tipo = "Produto", QuantidadeLotes = g.Count() })
                .ToListAsync();

            var topSalgados = await _context.Salgados
                .Where(s => s.Ativo)
                .GroupBy(s => s.Nome)
                .Select(g => new TopCadastroDTO { Nome = g.Key, Tipo = "Salgado", QuantidadeLotes = g.Count() })
                .ToListAsync();

            // Junta os dois e pega o "Top 5"
            var topCadastros = topProdutos.Concat(topSalgados)
                .OrderByDescending(x => x.QuantidadeLotes)
                .Take(5)
                .ToList();

            // 4. Gráfico de Categorias
            var catProdutos = await _context.Produtos
                .Where(p => p.Ativo)
                .Include(p => p.Categoria)
                .GroupBy(p => p.Categoria!.Nome)
                .Select(g => new { Categoria = g.Key, Qtd = g.Count() })
                .ToListAsync();

            var catSalgados = await _context.Salgados
                .Where(s => s.Ativo)
                .Include(s => s.Categoria)
                .GroupBy(s => s.Categoria!.Nome)
                .Select(g => new { Categoria = g.Key, Qtd = g.Count() })
                .ToListAsync();

            // Consolidando caso a mesma categoria tenha Salgados e Produtos
            var graficoCategorias = catProdutos.Concat(catSalgados)
                .GroupBy(x => x.Categoria)
                .Select(g => new GraficoCategoriaDTO
                {
                    Categoria = g.Key,
                    Quantidade = g.Sum(x => x.Qtd)
                })
                .OrderByDescending(g => g.Quantidade)
                .ToList();

            return new DashboardResumoDTO
            {
                TotalProdutosAtivos = totalProdutos,
                TotalSalgadosAtivos = totalSalgados,
                AlertasVencimento = alertas,
                TopCadastros = topCadastros,
                GraficoCategorias = graficoCategorias
            };
        }

        // --- MANTÉM OS SEUS CÓDIGOS ORIGINAIS INTACTOS ABAIXO ---
        public async Task<List<DashboardDTO>> GetAllAsync()
        {
            // O seu código original que ordena por Tipo -> Categoria -> Nome -> Id...
            var produtos = await _context.Produtos
                .Where(p => p.Ativo)
                .Include(p => p.Categoria)
                .Select(p => new DashboardDTO
                {
                    Id = p.Id,
                    Tipo = "Produto",
                    Nome = p.Nome,
                    Lote = p.Lote,
                    Categoria = p.Categoria != null ? p.Categoria.Nome : "Sem categoria",
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
                    Categoria = s.Categoria != null ? s.Categoria.Nome : "Sem categoria",
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

        public async Task<List<DashboardPrintDTO>> PrintEmCascataAsync(DashboardPrintRequestDTO request, int usuarioId)
        {
            // O seu código original de impressão cascata...
            var usuario = await _context.Usuarios.FindAsync(usuarioId)
                ?? throw new Exception("Usuário não encontrado");

            var dataManuseio = DateTime.Now;

            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => request.ProdutosIds.Contains(p.Id) && p.Ativo)
                .Select(p => new DashboardPrintDTO
                {
                    Nome = p.Nome,
                    Lote = p.Lote,
                    DataPrimaria = p.DataPrimaria,
                    DataManuseio = dataManuseio,
                    DataFinal = dataManuseio.AddDays(ValidadeDiasMapper.ParaDias(p.ValidadeDias)),
                    Responsavel = usuario.Nome,
                    Tipo = "Produto",
                    Id = p.Id
                })
                .ToListAsync();

            var salgados = await _context.Salgados
                .Include(s => s.Categoria)
                .Where(s => request.SalgadosIds.Contains(s.Id) && s.Ativo)
                .Select(s => new DashboardPrintDTO
                {
                    Nome = s.Nome,
                    Lote = s.Lote,
                    DataPrimaria = dataManuseio,
                    DataManuseio = dataManuseio.AddHours(ValidadeHorasMapper.ParaHoras(s.ValidadeHoras)),
                    DataFinal = dataManuseio
                        .AddHours(ValidadeHorasMapper.ParaHoras(s.ValidadeHoras))
                        .AddDays(ValidadeDiasMapper.ParaDias(s.ValidadeDias)),
                    Responsavel = usuario.Nome,
                    Tipo = "Salgado",
                    Id = s.Id
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