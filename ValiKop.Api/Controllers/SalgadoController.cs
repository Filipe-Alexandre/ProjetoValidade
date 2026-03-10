using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValiKop.Shared.DTOs.Salgado;
using System.Security.Claims;

[ApiController]
[Route("api/salgados")]
[Authorize]
public class SalgadosController : ControllerBase
{
    private readonly ISalgadoService _service;

    public SalgadosController(ISalgadoService service)
    {
        _service = service;
    }

    private int UsuarioId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    // --- ENDPOINT DE SUGESTÕES ---
    [HttpGet("sugestoes")]
    public async Task<IActionResult> ObterSugestoes()
    {
        var sugestoes = await _service.GetSugestoesAsync();
        return Ok(sugestoes);
    }

    // --- GET INATIVOS ---
    [HttpGet("inativos")]
    [Authorize(Roles = "Administrador")] // Mesma regra de segurança
    public async Task<IActionResult> GetInativos()
    {
        var result = await _service.GetInativosAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")] // Colocando regras Admin para manipulação
    public async Task<IActionResult> Create([FromBody] SalgadoFormDTO dto)
    {
        var result = await _service.CreateAsync(dto, UsuarioId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] SalgadoFormDTO dto)
    {
        try
        {
            var result = await _service.UpdateAsync(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    // --- PATCH PARA INATIVAR ---
    [HttpPatch("{id}/inativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Inativar(int id)
    {
        try
        {
            var result = await _service.InativarAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    // --- PATCH PARA REATIVAR ---
    [HttpPatch("{id}/reativar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Reativar(int id)
    {
        try
        {
            var result = await _service.ReativarAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    // --- DELETE DEFINITIVO ---
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> ExcluirDefinitivo(int id)
    {
        try
        {
            await _service.ExcluirDefinitivoAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpGet("imprimir/{id}")]
    public async Task<IActionResult> Print(int id)
    {
        var result = await _service.GetPrintAsync(id, UsuarioId);
        return result == null ? NotFound() : Ok(result);
    }
}