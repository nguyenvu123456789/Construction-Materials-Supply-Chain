using Application.Interfaces;
using Application.MappingProfile;
using Application.Validation;
using Application.Validation.ActivityLogs;
using Application.Validation.Auth;
using Domain.Interface;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Implementations;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// 1️⃣ AutoMapper configuration
// ==========================
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// ==========================
// 2️⃣ DbContext với AuditLogInterceptor
// ==========================
builder.Services.AddScoped<AuditLogInterceptor>();

builder.Services.AddDbContext<ScmVlxdContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<AuditLogInterceptor>();
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn"))
           .AddInterceptors(interceptor);
});

// ==========================
// 3️⃣ Đăng ký Repository
// ==========================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();
builder.Services.AddScoped<IMaterialCheckRepository, MaterialCheckRepository>();
builder.Services.AddScoped<IShippingLogRepository, ShippingLogRepository>();
builder.Services.AddScoped<IImportRepository, ImportRepository>();
builder.Services.AddScoped<IExportRepository, ExportRepository>();
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IPartnerRepository, PartnerRepository>();
builder.Services.AddScoped<IPartnerTypeRepository, PartnerTypeRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

// 4️⃣ Đăng ký Service
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IMaterialCheckService, MaterialCheckService>();
builder.Services.AddScoped<IShippingLogService, ShippingLogService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IImportDetailRepository, ImportDetailRepository>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IPartnerService, PartnerService>();


// ==========================
// 5️⃣ FluentValidation
// ==========================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ActivityLogPagedQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

// ==========================
// 6️⃣ Controllers, Swagger, CORS
// ==========================
builder.Services.AddControllers();
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

// 7️⃣ Seed Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ScmVlxdContext>();
    SeedData.Initialize(context);
}

// 8️⃣ Middleware pipeline
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
