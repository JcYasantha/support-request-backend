using Microsoft.Extensions.Options;
using SupportRequest.Core.Config;
using SupportRequest.Core.Interfaces;
using SupportRequest.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<SupportRequestConfig>(builder.Configuration.GetSection("SupportRequest"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<SupportRequestConfig>>().Value);
builder.Services.AddScoped<ITeamCapacityService, TeamCapacityService>();

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
