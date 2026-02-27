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

        // CREATE (ADMIN)
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Criar([FromBody] CategoriaFormDTO dto)
        {
            var categoria = await _categoriaService.AddAsync(dto);
            return Ok(categoria);
        }

        // READ - ALL
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var categorias = await _categoriaService.GetAllAsync();
            return Ok(categorias);
        }

        // READ - BY ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var categoria = await _categoriaService.GetByIdAsync(id);
            return Ok(categoria);
        }

        // UPDATE (ADMIN)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] CategoriaFormDTO dto)
        {
            var categoria = await _categoriaService.UpdateAsync(id, dto);
            return Ok(categoria);
        }

        // DELETE / INATIVAR (ADMIN)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Inativar(int id)
        {
            var categoria = await _categoriaService.InativarAsync(id);
            return Ok(categoria);
        }
    }
}
