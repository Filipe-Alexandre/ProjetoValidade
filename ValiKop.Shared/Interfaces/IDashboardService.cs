using ValiKop.Shared.DTOs.Dashboard;

namespace ValiKop.Shared.Interfaces
{
    public interface IDashboardService
    {
        Task<List<DashboardDTO>> GetAllAsync();

        Task<List<DashboardPrintDTO>> PrintEmCascataAsync(DashboardPrintRequestDTO request,int usuarioId);
    }
}
