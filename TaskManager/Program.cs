using ApplicationLayer.Services.TaskServices;
using CapaAplicacion.Services.DelegateServices;
using CapaAplicacion.Services.TaskServices;
using CapaInfraestructura.Repositorio.Delegates;
using CapaInfraestructura.Repositorio.Tasks;
using DomainLayer.Models;
using InfrastructureLayer;
using InfrastructureLayer.Repositorio.Commons;
using InfrastructureLayer.Repositorio.TaskRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskManagerContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagerDB"));
});

builder.Services.AddScoped<ITask, TaskRepository>();
builder.Services.AddScoped<IValidadorTareas, ValidadorTareas>();
builder.Services.AddScoped<ICalculadorTareas, CalculadorTareas>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddSingleton<ReactiveTaskQueue>();
builder.Services.AddControllers();
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

app.Run();
