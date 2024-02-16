using System.Net;
using System.Text;

namespace Api.Middleware
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggerMiddleware(
            RequestDelegate next
        )
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger _logger)
        {
            _logger.LogInformation($"Request: IP = {context.Request.Headers["x-forwarded-for"]}. Method = {context.Request.Method}. Path = {context.Request.Path}");

            DateTime starttime = DateTime.Now;
            
            try
            {
                context.Request.EnableBuffering();
                string requestBody = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync();
                context.Request.Body.Position = 0;
                _logger.LogInformation($"Request body: {requestBody}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception reading request: {ex.Message}");
            }

            Stream originalBody = context.Response.Body;
            try
            {
                using var memStream = new MemoryStream();
                context.Response.Body = memStream;

                await _next(context); 

                memStream.Position = 0;
                string responseBody = new StreamReader(memStream).ReadToEnd();

                memStream.Position = 0;
                await memStream.CopyToAsync(originalBody);
                _logger.LogInformation($"Response body: {responseBody}");
            }
            finally
            {
                context.Response.Body = originalBody;
            }

            var execTime = (DateTime.Now - starttime).TotalMilliseconds;
            
            _logger.LogInformation($"Response Status = {context.Response.StatusCode}. Execution time = {execTime}ms");
        }
    }


    public static class RequestLoggerMiddlewareExtentions
    { 
        public static IApplicationBuilder UseRequestLoggerMiddleware (this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggerMiddleware>();
        }
    }
}
