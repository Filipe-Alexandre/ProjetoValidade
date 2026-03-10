using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValiKop.Shared.DTOs.Produto;
using ValiKop.Shared.Interfaces;
using System.Security.Claims;

namespace ValiKop.Api.Controllers
{
    [ApiController]
    [Route("api/produtos")]
    [Authorize]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] ProdutoFormDTO dto)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var produto = await _produtoService.AddAsync(dto, usuarioId);
            return Ok(produto);
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var produtos = await _produtoService.GetAllAsync();
            return Ok(produtos);
        }

        // --- ENDPOINT DE SUGESTÕES ---
        [HttpGet("sugestoes")]
        public async Task<IActionResult> ObterSugestoes()
        {
            var sugestoes = await _produtoService.GetSugestoesAsync();
            return Ok(sugestoes);
        }

        // --- GET INATIVOS ---
        [HttpGet("inativos")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ListarInativos()
        {
            var produtos = await _produtoService.GetInativosAsync();
            return Ok(produtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var produto = await _produtoService.GetByIdAsync(id);
            return Ok(produto);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ProdutoFormDTO dto)
        {
            var produto = await _produtoService.UpdateAsync(id, dto);
            return Ok(produto);
        }

        // --- PATCH PARA INATIVAR ---
        [HttpPatch("{id:int}/inativar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Inativar(int id)
        {
            var produto = await _produtoService.InativarAsync(id);
            return Ok(produto);
        }

        // --- PATCH PARA REATIVAR ---
        [HttpPatch("{id:int}/reativar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Reativar(int id)
        {
            var produto = await _produtoService.ReativarAsync(id);
            return Ok(produto);
        }

        // --- DELETE DEFINITIVO ---
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ExcluirDefinitivo(int id)
        {
            await _produtoService.ExcluirDefinitivoAsync(id);
            return NoContent();
        }

        [HttpGet("categoria/{categoriaId:int}")]
        public async Task<IActionResult> ListarPorCategoria(int categoriaId)
        {
            var produtos = await _produtoService.GetByCategoriaIdAsync(categoriaId);
            return Ok(produtos);
        }

        [HttpGet("imprimir/{id:int}")]
        public async Task<IActionResult> Imprimir(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var produto = await _produtoService.GetParaImpressaoAsync(id, usuarioId);
            return Ok(produto);
        }
    }
}