using BusinessObjects;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Repositories.Interface;
using Repositories.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<ScmVlxdContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// Register DAO & Repository
builder.Services.AddScoped<MaterialDAO>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();

// Add services to the container
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
