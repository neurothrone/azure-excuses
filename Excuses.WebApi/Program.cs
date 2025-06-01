using Excuses.Persistence.EFCore.Data;
using Excuses.Persistence.EFCore.Repositories;
using Excuses.Persistence.Shared.Repositories;
using Excuses.WebApi.Endpoints;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ahh

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

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ExcusesDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (SqlException ex) when (ex.Message.Contains("already exists"))
    {
        // Log if you want
    }
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
