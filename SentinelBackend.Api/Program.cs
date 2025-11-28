using System.Security.Claims;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;   // ← ESTE
using Microsoft.IdentityModel.Tokens;                 // ← ESTE
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SentinelBackend.Domain.Ports;
using SentinelBackend.Infrastructure.Persistence;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using SentinelBackend.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// 1. Firebase Admin
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase-service-account.json")
});

// 2. DbContext + UnitOfWork
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Supabase")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// 3. Autenticación JWT con Firebase
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://securetoken.google.com/sentinel-peru-ed0ff";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://securetoken.google.com/sentinel-peru-ed0ff",
            ValidateAudience = true,
            ValidAudience = "sentinel-peru-ed0ff",
            ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var uid = context.Principal?.FindFirst("user_id")?.Value;
                if (string.IsNullOrEmpty(uid))
                    return;

                // Inyectamos el DbContext temporalmente para buscar el usuario
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var usuario = await dbContext.Usuarios
                    .FirstOrDefaultAsync(u => u.FirebaseUid == uid);

                if (usuario != null)
                {
                    // Añadimos los claims que necesitamos desde la DB real
                    var claims = new List<Claim>
                    {
                        new Claim("user_id", uid),
                        new Claim(ClaimTypes.Name, usuario.Nombre ?? usuario.Email),
                        new Claim(ClaimTypes.Role, usuario.Rol ?? "usuario"),
                        new Claim("role", usuario.Rol ?? "usuario"),
                        new Claim("db_user_id", usuario.Id.ToString()) // opcional: el Guid real
                    };

                    var identity = new ClaimsIdentity(claims, context.Scheme.Name);
                    context.Principal = new ClaimsPrincipal(identity);
                }
            }
        };
    });

builder.Services.AddAuthorization();

// 4. Controllers + Swagger
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Sentinel API", Version = "v1" });

    // ← AÑADE ESTO: configuración de seguridad para que aparezca el botón Authorize
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();   // ← IMPORTANTE
app.UseAuthorization();    // ← IMPORTANTE
app.UseMiddleware<CurrentUserMiddleware>();
app.MapControllers();

app.Run();