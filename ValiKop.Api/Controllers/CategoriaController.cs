using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValiKop.Shared.DTOs.Categoria;
using ValiKop.Shared.Interfaces;

namespace ValiKop.Api.Controllers
{
    [ApiController]
    [Route("api/categorias")]
    [Authorize] // usuário logado por padrão
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Criar([FromBody] CategoriaFormDTO dto)
        {
            var categoria = await _categoriaService.AddAsync(dto);
            return Ok(categoria);
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var categorias = await _categoriaService.GetAllAsync();
            return Ok(categorias);
        }

        // --- NOVO ROTEAMENTO: GET INATIVOS ---
        [HttpGet("inativos")]
        [Authorize(Roles = "Administrador")] // Só Admin vê a lixeira
        public async Task<IActionResult> ListarInativos()
        {
            var categorias = await _categoriaService.GetInativosAsync();
            return Ok(categorias);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var categoria = await _categoriaService.GetByIdAsync(id);
            return Ok(categoria);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] CategoriaFormDTO dto)
        {
            var categoria = await _categoriaService.UpdateAsync(id, dto);
            return Ok(categoria);
        }

        // --- AGORA É PATCH ---
        [HttpPatch("{id:int}/inativar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Inativar(int id)
        {
            var categoria = await _categoriaService.InativarAsync(id);
            return Ok(categoria);
        }

        // --- REATIVAR ---
        [HttpPatch("{id:int}/reativar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Reativar(int id)
        {
            var categoria = await _categoriaService.ReativarAsync(id);
            return Ok(categoria);
        }

        // --- HARD DELETE (O VERDADEIRO DELETE REST) ---
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ExcluirDefinitivo(int id)
        {
            await _categoriaService.DeleteAsync(id);
            return NoContent(); // 204 No Content é o padrão correto para um Delete de sucesso
        }
    }
}