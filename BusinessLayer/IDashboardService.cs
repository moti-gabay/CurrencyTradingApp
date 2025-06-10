// BusinessLayer/IDashboardService.cs
using SharedModels; // <--- שימוש ב-SharedModels כאן!
using System.Threading.Tasks;

namespace BusinessLayer
{
    public interface IDashboardService
    {
        Task<DashboardData> GetDashboardDataAsync();
    }
}