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

        // READ - ALL
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var produtos = await _produtoService.GetAllAsync();
            return Ok(produtos);
        }

        // READ - BY ID (FORM)
        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var produto = await _produtoService.GetByIdAsync(id);
            return Ok(produto);
        }

        // UPDATE (ADMIN)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] ProdutoFormDTO dto)
        {
            var produto = await _produtoService.UpdateAsync(id, dto);
            return Ok(produto);
        }

        // DELETE / INATIVAR (ADMIN)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Inativar(int id)
        {
            var produto = await _produtoService.InativarAsync(id);
            return Ok(produto);
        }

        // GET BY CATEGORY
        [HttpGet("categoria/{categoriaId:int}")]
        public async Task<IActionResult> ListarPorCategoria(int categoriaId)
        {
            var produtos = await _produtoService.GetByCategoriaIdAsync(categoriaId);
            return Ok(produtos);
        }

        // PRINT
        [HttpGet("imprimir/{id:int}")]
        public async Task<IActionResult> Imprimir(int id)
        {
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var produto = await _produtoService.GetParaImpressaoAsync(id, usuarioId);
            return Ok(produto);
        }
    }
}
