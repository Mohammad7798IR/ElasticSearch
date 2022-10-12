using DapperExample.Models.Entites;
using ElasticExample.Context;
using ElasticExample.Repositories;
using Microsoft.EntityFrameworkCore;
using Nest;
using ElasticExample.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

// Add services to the container.s

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddTransient<ElasticService>();

builder.Services
    .AddDbContextPool<ElasticDbContext>(options =>
{
    options.UseSqlServer(configuration.GetSection("ConnectionStrings")["DefaultConnectionStrings"]);
});

builder.Services.AddSingleton(a =>
new ElasticClient(
    new ConnectionSettings()
             .DisableDirectStreaming()
             //.DefaultIndex("example_users")
             .DefaultMappingFor<User>(a => a.IndexName("example-users4"))));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
