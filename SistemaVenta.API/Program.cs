using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SistemaVenta.IOC;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Cargar configuración explícitamente
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Servicios esenciales
builder.Services.AddControllers();

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sistema de Ventas API", Version = "v1" });

    // Configurar autenticación con JWT en Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Introduce el token en este formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

// Inyección de dependencias
builder.Services.InyectarDepedencias(builder.Configuration);

// Mostrar clave en consola para verificar que se lee correctamente
var jwtKey = builder.Configuration["Jwt:Key"];
Console.WriteLine($"JWT Key: {jwtKey}");

// Validar que la clave no sea null o vacía
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("La clave JWT no se encuentra en la configuración.");
}

// Configurar autenticación con JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPoliticaCors", policy =>
    {
        policy.AllowAnyOrigin()    // Permite cualquier origen (ajustar según seguridad)
              .AllowAnyMethod()    // Permite cualquier método (GET, POST, etc.)
              .AllowAnyHeader()
              .WithOrigins("http://10.0.2.2");   // Permite cualquier encabezado
    });
});

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema de Ventas API v1");
    });
}
app.Urls.Add("http://0.0.0.0:5185");

app.UseCors("NuevaPoliticaCors");
app.UseAuthentication();  // Autenticación antes de la autorización
app.UseAuthorization();

app.MapControllers();

app.Run();
