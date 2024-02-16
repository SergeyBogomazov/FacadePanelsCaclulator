using FacadeCalculator.Exceptions;
using Models;
using System.Net;

namespace Api.Middleware
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(
            RequestDelegate next
        )
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger _logger)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidFacadeException)
            {
                _logger.LogError($"CutProfilesForFacade: Invalid facade");
                context.Response.StatusCode = ((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (InvalidPanelException)
            {
                _logger.LogError($"CutProfilesForFacade: Invalid panel");
                context.Response.StatusCode = ((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (NotConvexFigure)
            {
                _logger.LogError($"CutProfilesForFacade: Figure isn`t convex");
                context.Response.StatusCode = ((int)HttpStatusCode.UnprocessableEntity);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message.ToString());

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }

    public static class ExceptionHandlerExtentions
    { 
        public static IApplicationBuilder UseCustomExceptionHandler (this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandler>();
        }
    }
}
