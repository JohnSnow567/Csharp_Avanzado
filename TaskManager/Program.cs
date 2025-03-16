using ApplicationLayer.Services.TaskServices;
using CapaAplicacion.Services.AuthServices;
using CapaAplicacion.Services.CacheServices;
using CapaAplicacion.Services.DelegateServices;
using CapaAplicacion.Services.TaskServices;
using CapaInfraestructura.Repositorio.Auth;
using CapaInfraestructura.Repositorio.Delegates;
using CapaInfraestructura.Repositorio.Tasks;
using DomainLayer.Models;
using InfrastructureLayer;
using InfrastructureLayer.Repositorio.Commons;
using Microsoft.IdentityModel.Tokens;
using InfrastructureLayer.Repositorio.TaskRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using TaskManager.Hubs;
using CapaInfraestructura.Repositorio.Cache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskManagerContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagerDB"));
});

builder.Services.AddScoped<ITask, TaskRepository>();
builder.Services.AddScoped<IValidadorTareas, ValidadorTareas>();
builder.Services.AddScoped<ICalculadorTareas, CalculadorTareas>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSingleton<ReactiveTaskQueue>();
builder.Services.AddControllers();

builder.Services.AddSignalR();

// Configuración de la clave secreta (idealmente en appsettings.json)
var secretKey = builder.Configuration["Jwt:SecretKey"];

if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
{
    throw new Exception("JWT SecretKey is too short. It must be at least 32 characters long.");
}

var key = Encoding.ASCII.GetBytes(secretKey);

// Configuración de la autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // En producción, establecer a true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSingleton<IAuthService>(new AuthService(secretKey));



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<TaskManagerContext>();
//    context.Database.Migrate();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<TareasHub>("/tareasHub");

app.Run();
