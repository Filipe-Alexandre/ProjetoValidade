using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ValiKop.Shared.Interfaces;
using System.Security.Claims;

namespace ValiKop.Api.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var itens = await _dashboardService.GetAllAsync();
            return Ok(itens);
        }

        [HttpPost("imprimir")]
        public async Task<IActionResult> PrintEmCascata([FromBody] DashboardPrintRequestDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId))
                return Unauthorized("Usuário não autenticado ou inválido.");

            var result = await _dashboardService.PrintEmCascataAsync(request, usuarioId);

            return Ok(result);
        }

    }
}
