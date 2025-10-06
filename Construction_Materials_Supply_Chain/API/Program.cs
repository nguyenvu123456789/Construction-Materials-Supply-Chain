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
builder.Services.AddScoped<ImportDAO>();
builder.Services.AddScoped<ExportDAO>();
builder.Services.AddScoped<RoleDAO>();

// Repository
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IShippingLogRepository, ShippingLogRepository>();
builder.Services.AddScoped<ISupplyChainRepository, SupplyChainRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IImportRepository, ImportRepository>();
builder.Services.AddScoped<IExportRepository, ExportRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Audit Log Interceptor
builder.Services.AddScoped<AuditLogInterceptor>();

builder.Services.AddDbContext<ScmVlxdContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<AuditLogInterceptor>();
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn"))
           .AddInterceptors(interceptor);
});

// Add services to the container
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ScmVlxdContext>();
SeedData.Initialize(context);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
