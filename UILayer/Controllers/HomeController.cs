using BusinessLayer; // הוסף את ה-using הזה
using Microsoft.AspNetCore.Mvc;
using SharedModels; // הוסף את ה-using הזה
using UILayer.Models; // וודא שזה קיים אם יש לך ViewModel
using System.Diagnostics;

namespace UILayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _dashboardService; // הוסף את זה

        // עדכן את הבנאי כדי לקבל את IDashboardService
        public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService; // אתחל את השירות
        }

        public async Task<IActionResult> Index()
        {
            // קבל את נתוני הדאשבורד מהשירות החדש
            var dashboardData = await _dashboardService.GetDashboardDataAsync();
            return View(dashboardData); // העבר את המודל לתצוגה
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}