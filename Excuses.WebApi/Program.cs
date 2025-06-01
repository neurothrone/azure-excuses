using Excuses.Persistence.EFCore.Data;
using Excuses.Persistence.EFCore.Repositories;
using Excuses.Persistence.Shared.Repositories;
using Excuses.WebApi.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// !: InMemory
//builder.Services.AddSingleton<InMemoryDataStore>();
//builder.Services.AddScoped<IExcuseRepository, ExcuseInMemoryRepository>();

// !: EFCore SQLite
//builder.Services.AddDbContext<ExcusesDbContext>(options => options.UseSqlite("Data Source=excuses.db"));

// !: EFCore SQLServer
builder.Services.AddDbContext<ExcusesDbContext>(options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
    ));

builder.Services.AddScoped<IExcuseRepository, ExcusesEfCoreRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapGet("/", context =>
{
    context.Response.Redirect("/index.html");
    return Task.CompletedTask;
});

app.MapExcuseEndpoints();

app.Run();
