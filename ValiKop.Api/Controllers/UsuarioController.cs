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
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, UsuarioFormDTO dto)
    {
        int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var usuario = await _usuarioService.UpdateAsync(id, dto, adminId);
        return Ok(usuario);
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Inativar(int id)
    {
        int adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _usuarioService.InativarAsync(id, adminId);
        return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost("resetar-senha/{id}")]
    public async Task<IActionResult> ResetarSenha(int id)
    {
        int adminId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var response = await _usuarioService.ResetarSenhaAsync(id, adminId);

        return Ok(response);
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    public async Task<IActionResult> ListarUsuarios()
    {
        var usuarios = await _usuarioService.GetAllAsync();
        return Ok(usuarios);
    }

    // ADMIN — BUSCAR POR ID
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
}
