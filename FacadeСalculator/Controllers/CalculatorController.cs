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
        private const int calculationTimeoutMilliSeconds = 3000;

        private readonly ILogger<CalculatorController> _logger;

        public CalculatorController(ILogger<CalculatorController> logger)
        {
            _logger = logger;
        }

        [HttpPost("cut")]
        public async Task<IActionResult> CutProfilesForFacade([FromBody] FacadeData data)
        {
            _logger.LogInformation($"CutProfilesForFacade: Try to calculate profiles for facade = {string.Join<ApiModels.Point>(',', data.Profile)}");

            var calculator = new Calculator();

            var calcTask = calculator.GetPanelsToCoverProfile(data.Profile.Select(p => new Models.Point(p.X, p.Y)).ToArray(), defaultPanelSize);
            Panel[] result;

            var task = Task.WhenAny(calcTask, Task.Delay(calculationTimeoutMilliSeconds));

            try
            {
                if (await task == calcTask)
                {
                    result = calcTask.Result;
                }
                else
                {
                    _logger.LogCritical($"GetPanelsToCoverProfile: timeout");
                    return StatusCode((int)HttpStatusCode.InternalServerError);
                };
            }
            catch (InvalidFacadeException)
            {
                if (task.Exception != null)
                {
                    _logger.LogError($"CutProfilesForFacade: Invalid Facade");
                    return StatusCode((int)HttpStatusCode.UnprocessableEntity);
                }

                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            var panelHeights = result.Select(p => p.size.Height);

            _logger.LogInformation($"CutProfilesForFacade: Result = {string.Join(',', panelHeights)}");

            return Ok(new CutProfilesResponse() { 
                Heights = panelHeights
            });
        }

        private readonly Size defaultPanelSize = new Size(500f, 13500f);
    }
}