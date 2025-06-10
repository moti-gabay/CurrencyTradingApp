using Microsoft.AspNetCore.Mvc;
using BusinessLayer; // <--- שינוי: זהו מרחב השמות הנכון עבור SimulationService
using SharedModels; // <--- שינוי: זהו מרחב השמות הנכון עבור ITradeNotifier
using Microsoft.Extensions.Logging;

namespace UILayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // זה יגדיר את ה-base route כ- /api/Simulation
    public class SimulationController : ControllerBase
    {
        private readonly ISimulationService _simulationService; // <--- שינוי: השתמש ב-ISimulationService
        private readonly ILogger<SimulationController> _logger;

        // הזרקת תלויות: ISimulationService ו-Logger
        public SimulationController(ISimulationService simulationService, ILogger<SimulationController> logger) // <--- שינוי: הזרק ISimulationService
        {
            _simulationService = simulationService;
            _logger = logger;
        }

        // Endpoint להתחלת הסימולציה
        // ה-URL המלא יהיה /api/Simulation/start
        [HttpPost("start")] // אנחנו מצפים לבקשת POST מכפתור ה-Start Simulation
        public IActionResult StartSimulation()
        {
            _logger.LogInformation("StartSimulation API endpoint hit.");

            try
            {
                _simulationService.StartSimulation(); // <--- שינוי: קרא למתודה הנכונה ב-SimulationService
                _logger.LogInformation("Simulation started successfully via API.");
                return Ok("Simulation started successfully."); // מחזיר 200 OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting simulation via API.");
                return StatusCode(500, $"Error starting simulation: {ex.Message}"); // מחזיר 500 Internal Server Error
            }
        }

        // Endpoint לעצירת הסימולציה (אם רלוונטי)
        // ה-URL המלא יהיה /api/Simulation/stop
        [HttpPost("stop")]
        public IActionResult StopSimulation()
        {
            _logger.LogInformation("StopSimulation API endpoint hit.");
            try
            {
                _simulationService.StopSimulation(); // <--- שינוי: קרא למתודה הנכונה ב-SimulationService
                _logger.LogInformation("Simulation stopped successfully via API.");
                return Ok("Simulation stopped."); // מחזיר 200 OK
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping simulation via API.");
                return StatusCode(500, $"Error stopping simulation: {ex.Message}");
            }
        }
    }
}