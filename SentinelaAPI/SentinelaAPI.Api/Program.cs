using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SentinelaAPI.Api.Middleware;
using SentinelaAPI.Application.Interfaces;
using SentinelaAPI.Application.Services;
using SentinelaAPI.Domain.Interfaces;
using SentinelaAPI.Infrastructure.Data;
using SentinelaAPI.Infrastructure.Repositories;
using SQLitePCL;
using System.Reflection;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        if (!string.IsNullOrEmpty(databaseUrl))
        {
            var uri = new Uri(databaseUrl);
            var userInfo = uri.UserInfo.Split(':');
            var npgsqlConnection = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(npgsqlConnection));
        }
        else
        {
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));
        }

        builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        builder.Services.AddScoped<IAuditLogService, AuditLogService>();
        builder.Services.AddScoped<IAnomalyAlertRepository, AnomalyAlertRepository>();
        builder.Services.AddScoped<IAnomalyDetectionService, AnomalyDetectionService>();
        builder.Services.AddHttpClient<CveMonitorService>();
        builder.Services.AddScoped<ICveMonitorService, CveMonitorService>();

        var app = builder.Build();

        // Apply migrations automatically on startup
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }

        // Enable Swagger in all environments for portfolio visibility
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SentinelaAPI v1");
            c.RoutePrefix = string.Empty;
        });

        app.UseMiddleware<AuditMiddleware>();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}