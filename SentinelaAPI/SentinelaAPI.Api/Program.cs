using Microsoft.EntityFrameworkCore;
using SentinelaAPI.Api.Middleware;
using SentinelaAPI.Application.Interfaces;
using SentinelaAPI.Application.Services;
using SentinelaAPI.Domain.Interfaces;
using SentinelaAPI.Infrastructure.Data;
using SentinelaAPI.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAnomalyAlertRepository, AnomalyAlertRepository>();
builder.Services.AddScoped<IAnomalyDetectionService, AnomalyDetectionService>();
builder.Services.AddHttpClient<CveMonitorService>();
builder.Services.AddScoped<ICveMonitorService, CveMonitorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<AuditMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();