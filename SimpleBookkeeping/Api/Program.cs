using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SimpleBookkeeping.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite database
builder.Services.AddDbContext<SqliteDbContext>(options =>
    options.UseSqlite("Data Source=bookkeeping.db"));

// CORS 配置，允许小程序调用
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMiniProgram", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowMiniProgram");
app.UseAuthorization();
app.MapControllers();

app.Run();
