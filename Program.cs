using LojaApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// EF Core + SQLite
builder.Services.AddDbContext<LojaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Loja API",
        Version = "v1",
        Description = "API para gerenciamento de Pessoas, Endereços, Categorias, Produtos e Tags com EF Core."
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Swagger sempre habilitado (facilita avaliação/apresentação)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Loja API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz: http://localhost:PORT/
});

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
