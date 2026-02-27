using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValiKop.Shared.DTOs.Auth;
using ValiKop.Shared.DTOs.Usuario;
using ValiKop.Shared.Interfaces;
using System.Security.Claims;

namespace ValiKop.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // LOGIN
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return Unauthorized("Usuário ou senha inválidos.");

            return Ok(result);
        }

        // ALTERAR SENHA (usuário logado)
        [HttpPut("alterar-senha")]
        [Authorize]
        public async Task<IActionResult> AlterarSenha([FromBody] UsuarioAlterarSenhaDTO dto)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (usuarioIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(usuarioIdClaim.Value);

            await _authService.AlterarSenhaAsync(usuarioId, dto);

            return NoContent();
        }

        // LOGOUT (dummy)
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok();
        }
    }
}
