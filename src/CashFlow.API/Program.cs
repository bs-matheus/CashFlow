using CashFlow.API.Filters;
using CashFlow.API.Middleware;
using CashFlow.Application;
using CashFlow.Infrastructure;
using CashFlow.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace CashFlow.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = @"JWT Authorization header using the Bearer scheme.
                                Enter 'Bearer' [space] and then your token in the text input below.
                                Example: 'Bearer 12345abcdef'",
                In = ParameterLocation.Header,
                Scheme = "Bearer",
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });
        });

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        builder.Services.AddMvc(options => options.Filters.Add(typeof(ExceptionFilter)));

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication();

        string signingKey = builder.Configuration.GetValue<string>("Settings:Jwt:SigningKey")!;

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = new TimeSpan(0),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
            };
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<CultureMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();
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
