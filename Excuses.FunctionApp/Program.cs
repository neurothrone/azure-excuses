using Excuses.Persistence.EFCore.Data;
using Excuses.Persistence.EFCore.Repositories;
using Excuses.Persistence.Shared.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// Ahh

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddDbContext<ExcusesDbContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
    ));

builder.Services.AddScoped<IExcuseRepository, ExcusesEfCoreRepository>();

builder.Build().Run();
