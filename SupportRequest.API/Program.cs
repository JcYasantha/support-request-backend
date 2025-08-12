using Microsoft.Extensions.Options;
using SupportRequest.Core.Config;
using SupportRequest.Core.InMemory;
using SupportRequest.Core.Interfaces.Repository;
using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Service;
using SupportRequest.Service.BackgroundServices;
using SupportRequest.Service.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<SupportRequestConfig>(builder.Configuration.GetSection("SupportRequest"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<SupportRequestConfig>>().Value);

builder.Services.AddScoped<ITeamsRepository, InMemoryTeamRepository>();
builder.Services.AddScoped<ISupportRequestSessionRepository, InMemorySupportRequestSessionRepository>();
builder.Services.AddScoped<ITeamCapacityService, TeamCapacityService>();
builder.Services.AddScoped<ISupportRequestQueueService, SupportRequestQueueService>();
builder.Services.AddScoped<ISupportRequestSessionService, SupportRequestSessionService>();

builder.Services.AddScoped<IAgentAssignmentStratergy, RoundRobinAssignmentStratergy>();
builder.Services.AddScoped<ISupportRequestAssignmentService, SupportRequestAssignmentService>();

builder.Services.AddHostedService<RequestAssignmentBackgroundService>();

builder.Services.AddSingleton<InMemorySupportRequestQueueStore>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
