using System.Text;
using AplicationLayer.Dtos.Account.JWT;
using AplicationLayer.Helper;
using AplicationLayer.Interfaces.Service;
using AplicationLayer.Repository.ICommon;
using AplicationLayer.Service;
using DomainLayer.Models;
using DomainLayer.Settings;
using InfrastructureLayer.Data;
using InfrastructureLayer.Model;
using InfrastructureLayer.Repositories.TaskRepository;
using InfrastructureLayer.Seeds;
using InfrastructureLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<TaskDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("TaskConection")));
builder.Services.AddDbContext<IdentityContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});

//IdentityLogic
#region Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();
#endregion
#region JWT
builder.Services.Configure<JWTSetting>(builder.Configuration.GetSection("JWTSettings"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
        ValidAudience = builder.Configuration["JWTSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:Key"]))
    };
    options.Events = new JwtBearerEvents()
    {
        OnAuthenticationFailed = c =>
        {
            c.NoResult();
            if (!c.Response.HasStarted)
            {
                c.Response.StatusCode = 500;
                c.Response.ContentType = "text/plain";
                return c.Response.WriteAsync(c.Exception.ToString());
            }
            return Task.CompletedTask;
        },


        OnChallenge = c =>
        {
            if (!c.Response.HasStarted)
            {
                c.HandleResponse();
                c.Response.StatusCode = 401;
                c.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new JWTResponse { HasError = true, Error = "An unexpected authentication error occurred" });
                return c.Response.WriteAsync(result);
            }
            return Task.CompletedTask;
        }
,
        OnForbidden = c =>
        {
            if (!c.Response.HasStarted)
            {
                c.Response.StatusCode = 403;
                c.Response.ContentType = "application/json";
                var result = JsonConvert.SerializeObject(new JWTResponse { HasError = true, Error = "You aren't Authorized to access to this content" });
                return c.Response.WriteAsync(result);
            }
            return Task.CompletedTask;
        }

    };

});
#endregion

//IdentityServices
#region Services
builder.Services.AddScoped<IAccountService, AccountService>();
#endregion





//Add Services
builder.Services.AddScoped<ICommonProcess<Tarea>, TareaRepositorio>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<TaskHelper>();
builder.Services.AddSignalR();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await services.seedDatabaseAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseWhen(context => !context.Request.Path.StartsWithSegments("/swagger"), appBuilder =>
{
    appBuilder.UseAuthentication();
    appBuilder.UseAuthorization();
});

app.MapControllers();

app.Run();
