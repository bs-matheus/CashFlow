using CashFlow.API.Filters;
using CashFlow.API.Middleware;
using CashFlow.Application;
using CashFlow.Infrastructure;
using CashFlow.Infrastructure.Migrations;

namespace CashFlow.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<CultureMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        await MigrateDatabaseAsync(app);

        app.Run();
    }

    private static async Task MigrateDatabaseAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await DatabaseMigration.MigrateDatabaseAsync(scope.ServiceProvider);
    }
}
