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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody]SalgadoFormDTO dto)
    {
        var result = await _service.CreateAsync(dto, UsuarioId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id,[FromBody] SalgadoFormDTO dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
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


    [HttpGet("imprimir/{id}")]
    public async Task<IActionResult> Print(int id)
    {
        var result = await _service.GetPrintAsync(id, UsuarioId);
        return result == null ? NotFound() : Ok(result);
    }
}
