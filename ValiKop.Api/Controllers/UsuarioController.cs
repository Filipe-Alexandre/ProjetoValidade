using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValiKop.Shared.DTOs.Usuario;
using ValiKop.Shared.Interfaces;
using System.Security.Claims;

namespace ValiKop.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Criar(UsuarioFormDTO dto)
    {
        int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await _usuarioService.AddAsync(dto, adminId);
        return Ok(usuario);
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> ListarUsuarios()
    {
        var usuarios = await _usuarioService.GetAllAsync();
        return Ok(usuarios);
    }

    // --- LISTAR INATIVOS ---
    [Authorize(Roles = "Administrador")]
    [HttpGet("inativos")]
    public async Task<IActionResult> ListarInativos()
    {
        var usuarios = await _usuarioService.GetInativosAsync();
        return Ok(usuarios);
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var usuario = await _usuarioService.GetByIdAsync(id);
        if (usuario == null)
            return NotFound();

        return Ok(usuario);
    }

    [HttpGet("me")]
    public async Task<IActionResult> MeuUsuario()
    {
        int usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await _usuarioService.GetByIdAsync(usuarioId);
        return Ok(usuario);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, UsuarioFormDTO dto)
    {
        int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await _usuarioService.UpdateAsync(id, dto, adminId);
        return Ok(usuario);
    }

    // --- PATCH PARA INATIVAR ---
    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id}/inativar")]
    public async Task<IActionResult> Inativar(int id)
    {
        try
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _usuarioService.InativarAsync(id, adminId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // --- PATCH PARA REATIVAR ---
    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id}/reativar")]
    public async Task<IActionResult> Reativar(int id)
    {
        try
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _usuarioService.ReativarAsync(id, adminId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    // --- DELETE DEFINITIVO ---
    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> ExcluirDefinitivo(int id)
    {
        try
        {
            int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _usuarioService.ExcluirDefinitivoAsync(id, adminId);
            return NoContent();
        }
        catch (Exception ex) // Vai cair aqui se o usuário tiver criado produtos!
        {
            return BadRequest(new { mensagem = "Não é possível excluir este usuário pois ele possui vínculos no sistema. Use a opção de Inativar." });
        }
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost("resetar-senha/{id}")]
    public async Task<IActionResult> ResetarSenha(int id)
    {
        int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response = await _usuarioService.ResetarSenhaAsync(id, adminId);
        return Ok(response);
    }
}