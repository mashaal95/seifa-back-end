using Microsoft.EntityFrameworkCore;
using Seifa2011_2016_API.Interfaces;
using Seifa2011_2016_API.Models;
using Seifa2011_2016_API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEntityFrameworkInMemoryDatabase();

builder.Services.AddDbContext<TestDbContext>(option =>
    option.UseSqlServer("Data Source=localhost;Initial Catalog=TestDB;User id=sa;Password=7Times=10!;TrustServerCertificate=Yes;MultipleActiveResultSets=true"));


// Register ISeifaRepository
builder.Services.AddScoped<ISeifaRepository, SeifaRepository>();



var app = builder.Build();

app.UseCors(
    options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
);

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
