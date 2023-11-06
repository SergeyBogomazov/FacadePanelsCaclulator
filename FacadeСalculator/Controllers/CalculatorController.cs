using ApiModels;
using FacadeCalculator;
using FacadeCalculator.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Net;

namespace FacadeСalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : ControllerBase
    {
        private readonly ILogger<CalculatorController> _logger;

        public CalculatorController(ILogger<CalculatorController> logger)
        {
            _logger = logger;
        }

        [HttpPost("facadePanels")]
        public async Task<IActionResult> CutProfilesForFacade([FromBody] FacadeData data)
        {
            _logger.LogInformation($"CutProfilesForFacade: Try to calculate profiles for facade = {string.Join<ApiModels.Point>(',', data.Profile)} with panelSize = {data.PanelSize}");

            var calculator = new Calculator();

            IEnumerable<Panel> result;

            try
            {
                result = await calculator.GetPanelsToCoverProfile(data.Profile.Select(p => new Models.Point(p.X, p.Y)).ToArray(), data.PanelSize);
            }
            catch (InvalidFacadeException)
            {
                _logger.LogError($"CutProfilesForFacade: Invalid facade");
                return StatusCode((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (InvalidPanelException)
            {
                _logger.LogError($"CutProfilesForFacade: Invalid panel");
                return StatusCode((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (NotConvexFigure)
            {
                _logger.LogError($"CutProfilesForFacade: Figure isn`t convex");
                return StatusCode((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"CutProfilesForFacade: {ex.GetType}, {ex.Message}");
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            var panelHeights = result.Select(p => p.size.Height);

            _logger.LogInformation($"CutProfilesForFacade: Result = {string.Join(',', panelHeights)}");

            return Ok(new CutProfilesResponse() { 
                Heights = panelHeights
            });
        }
    }
}