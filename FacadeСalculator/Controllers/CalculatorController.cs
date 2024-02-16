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
        public async Task<IActionResult> CutProfilesForFacade([FromBody] FacadeData data, [FromServices] ICalculator calculator)
        {
            _logger.LogInformation($"CutProfilesForFacade: Try to calculate profiles for facade = {string.Join<ApiModels.Point>(',', data.Profile)} with panelSize = {data.PanelSize}");

            var result = calculator.GetPanelsToCoverProfile(data.Profile.Select(p => new Models.Point(p.X, p.Y)).ToArray(), data.PanelSize);

            return Ok(new CutProfilesResponse(result.Select(p => p.size.Height)));
        }
    }
}