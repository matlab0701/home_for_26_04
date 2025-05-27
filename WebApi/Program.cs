using System.Text;
using Domain.Entities;
using Hangfire;
using Hangfire.PostgreSql;
using Infrastructure.AutoMapper;
using Infrastructure.Data;
using Infrastructure.Extensions;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();


// builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(InfrastructureProfile));

builder.Services.AddScoped<IBaseRepository<Category, int>, CategoryRepository>();
builder.Services.AddScoped<IBaseRepository<Product, int>, ProductRepository>();
builder.Services.AddScoped<IBaseRepository<Order, int>, OrderRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddMemoryCache();
// builder.Services.AddScoped<IMemoryCashService, MemoryCashService>();
builder.Services.AddScoped<IPoductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddHangfire(opt => opt.UsePostgreSqlStorage(connection));
builder.Services.AddHangfireServer();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseNpgsql(connection));

builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(config =>
    {
        config.Password.RequiredLength = 4;
        config.Password.RequireDigit = false;
        config.Password.RequireNonAlphanumeric = false;
        config.Password.RequireUppercase = false;
        config.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "Описание вашего API",
        Contact = new OpenApiContact
        {
          
        }
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен: Bearer {your token}"
    });


    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
}

// app.Services.RegisterRecurringJobs();
app.UseHangfireServer();

app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
