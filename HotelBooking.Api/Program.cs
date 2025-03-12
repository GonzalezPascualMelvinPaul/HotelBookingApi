using HotelBooking.Api.Services;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/hotelbooking.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new ArgumentNullException("JwtSettings:Secret no est configurado."));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true
        };
    });

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PuedeReservar", policy => policy.RequireAuthenticatedUser());

    options.AddPolicy("PuedeCancelar", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "Admin") ||
            context.User.HasClaim(c => c.Type == "UserId")
        ));

    options.AddPolicy("EsUsuarioFrecuente", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "ReservasHechas" && int.Parse(c.Value) > 5)
        ));

    options.AddPolicy("CheckInEnRango", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "CheckInDate") &&
            DateTime.TryParse(context.User.FindFirst("CheckInDate").Value, out var checkInDate) &&
            checkInDate <= DateTime.UtcNow.AddMonths(6)
        ));

    options.AddPolicy("EsAdmin", policy => policy.RequireRole("Admin"));

    options.AddPolicy("PuedeVerHabitaciones", policy => policy.RequireAuthenticatedUser());

    options.AddPolicy("VerHabitacionesVIP", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == ClaimTypes.Email && c.Value.EndsWith("@empresa.com"))
        ));

    options.AddPolicy("EditarHabitacionesPremium", policy =>
        policy.RequireRole("Admin", "Gerente"));
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token en el formato: Bearer {tu_token}"
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
            new string[] {}
        }
    });
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 403)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{ \"status\": 403, \"message\": \"No tienes permisos para acceder a este recurso.\" }");
    }
    else if (context.Response.StatusCode == 401)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{ \"status\": 401, \"message\": \"No ests autenticado. Por favor, inicia sesin.\" }");
    }
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Iniciando la API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicacin fall al iniciar.");
}
finally
{
    Log.CloseAndFlush();
}