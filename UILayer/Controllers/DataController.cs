using Microsoft.AspNetCore.Mvc;
using BusinessLayer; // וודא שזה קיים עבור IDashboardService
using SharedModels; // וודא שזה קיים עבור DashboardData, CurrencyPair, DashboardSummary
using System.Threading.Tasks;

namespace UILayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DataController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentDashboardData()
        {
            try
            {
                // שינוי: קוראים למתודה שמחזירה את כל נתוני הדאשבורד באובייקט אחד
                var dashboardData = await _dashboardService.GetDashboardDataAsync(); // <--- שינוי: קורא ל-GetDashboardDataAsync

                if (dashboardData == null)
                {
                    return NotFound("No current dashboard data available.");
                }

                // החזר 200 OK עם אובייקט DashboardData בפורמט JSON
                // ה-JavaScript בצד הלקוח יצטרך לפרש את המבנה הזה.
                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error in GetCurrentDashboardData: {ex.Message}");
                return StatusCode(500, "Internal server error: Could not retrieve dashboard data.");
            }
        }
    }
}