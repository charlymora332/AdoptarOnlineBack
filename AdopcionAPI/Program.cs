using AdopcionOnline.Infrastructure;
using AdopcionOnline.Infrastructure.Repositories;
using Application.Archivos.Helpers;
using Application.Archivos.Intefaces;
using Application.Auth.Intefaces;
using Application.Auth.Services;
using Application.IA.Intefaces;
using Application.IA.Services;
using Application.Mascotas.Intefaces;
using Application.Mascotas.Services;
using DotNetEnv;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// 🔹 Conexión a la base de datos
// -----------------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

// -----------------------------------------------------------------------------
// 🔹 CORS (Next.js)
// -----------------------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("NextPolicy", cors =>
    {
        cors.WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000",
                "http://localhost:3001",
                "http://127.0.0.1:3001",
                "https://localhost:3000",
                "https://localhost:3001"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// -----------------------------------------------------------------------------
// 🔹 Repositorios
// -----------------------------------------------------------------------------
builder.Services.AddScoped<IMascotaRepository, MascotaRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
//builder.Services.AddScoped<IMascotaAdminRepository, MascotaAdminRepository>();

// -----------------------------------------------------------------------------
// 🔹 Servicios
// -----------------------------------------------------------------------------
builder.Services.AddScoped<IMascotaService, MascotaService>();
builder.Services.AddScoped<IImagenService, ImagenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpClient<IIaDescripcionService, IaDescripcionService>();
builder.Services.AddHttpContextAccessor();

// -----------------------------------------------------------------------------
// 🔹 Servicio IA
// -----------------------------------------------------------------------------
builder.Services.AddHttpClient<IIaDescripcionService, IaDescripcionService>(client =>
{
    var apiKey = builder.Configuration["OpenAI:ApiKey"];
    if (!string.IsNullOrWhiteSpace(apiKey))
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});

// -----------------------------------------------------------------------------
// 🔹 Controllers y Swagger
// -----------------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Application.Mascotas.Services.MascotaService).Assembly));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Esto agrega la definición del JWT en Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingrese el token JWT como: Bearer {token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// -----------------------------------------------------------------------------
// ✅ Crear base de datos al iniciar
// -----------------------------------------------------------------------------
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false; // true en producción
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ClockSkew = TimeSpan.Zero
//    };
//});
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // true en producción
    options.SaveToken = true;

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("jwt"))
            {
                context.Token = context.Request.Cookies["jwt"];
            }
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// -----------------------------------------------------------------------------
// 🔹 Construir app
// -----------------------------------------------------------------------------
var app = builder.Build();

UrlImgCompleta.Configure(
    app.Services.GetRequiredService<IHttpContextAccessor>(),
    app.Configuration
);

// -----------------------------------------------------------------------------
// 🔹 Configuración de carpeta Uploads
// -----------------------------------------------------------------------------
var relativeUploadFolder = builder.Configuration["UploadFolder"];
if (string.IsNullOrWhiteSpace(relativeUploadFolder))
    throw new Exception("❌ ERROR: No se encontró 'UploadFolder' en appsettings.json");

var uploadFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), relativeUploadFolder));

if (!Directory.Exists(uploadFolder))
{
    Directory.CreateDirectory(uploadFolder);
    Console.WriteLine("📁 Carpeta de uploads creada en: " + uploadFolder);
}

// Servir archivos estáticos desde la carpeta Uploads
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadFolder),
    RequestPath = "/Storege/Uploads"
});

// -----------------------------------------------------------------------------
// 🔹 CORS
// -----------------------------------------------------------------------------
app.UseCors("NextPolicy");

// -----------------------------------------------------------------------------
// 🔹 Swagger (solo en desarrollo)
// -----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

// -----------------------------------------------------------------------------
// 🔹 HTTPS y Authorization
// -----------------------------------------------------------------------------
app.UseHttpsRedirection();
app.UseAuthentication(); // <--- obligatorio

app.UseAuthorization();

app.Use(async (context, next) =>
{
    try
    {
        await next(); // Ejecuta el siguiente middleware
    }
    catch (Exception ex)
    {
        Console.WriteLine("💥 Excepción capturada globalmente:");
        Console.WriteLine(ex);

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            error = ex.Message,
            detalle = ex.InnerException?.Message
        });
    }
});

// -----------------------------------------------------------------------------
// 🔹 Mapear Controllers
// -----------------------------------------------------------------------------
app.MapControllers();

app.Run();