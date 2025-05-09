using AplicationLayer.Helper;
using AplicationLayer.Repository.ICommon;
using AplicationLayer.Service;
using DomainLayer.Models;
using InfrastructureLayer.Data;
using InfrastructureLayer.Repositories.TaskRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TaskConection")));

//Add Services
builder.Services.AddScoped<ICommonProcess<Tarea>, TareaRepositorio>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TaskHelper>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

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
