using Api.Middleware;
using FacadeCalculator;

namespace FacadeСalculator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

            builder.Services.AddSingleton<ICalculator, Calculator>();

            var app = builder.Build();
            

            app.UseRequestLoggerMiddleware();
            app.UseCustomExceptionHandler();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();

            app.Run();
        }
    }
}