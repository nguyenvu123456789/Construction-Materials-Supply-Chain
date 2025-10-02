using API.Profiles;
using BusinessObjects;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Interface;
using Repositories.Repositories;

var builder = WebApplication.CreateBuilder(args);

// AutoMapper configuration
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// Register DbContext
builder.Services.AddDbContext<ScmVlxdContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

// DAO
builder.Services.AddScoped<ActivityLogDAO>();
builder.Services.AddScoped<MaterialDAO>();
builder.Services.AddScoped<NotificationDAO>();
builder.Services.AddScoped<PermissionDAO>();
builder.Services.AddScoped<RoleDAO>();
builder.Services.AddScoped<ShippingLogDAO>();
builder.Services.AddScoped<SupplyChainDAO>();
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<UserRoleDAO>();

// Repository
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IShippingLogRepository, ShippingLogRepository>();
builder.Services.AddScoped<ISupplyChainRepository, SupplyChainRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


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
