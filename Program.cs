using Scalar.AspNetCore;
using ExamApiDemo.Interfaces;
using ExamApiDemo.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IExamResultService, ExamResultService>();
// Register SQL Connection
// builder.Services.AddSingleton<IConfiguration>(
//     builder.Configuration
// );
var connString = builder.Configuration.GetConnectionString("ExamDb")
    ?? throw new InvalidOperationException("Connection string 'ExamDb' not found.");
builder.Services.AddSingleton(connString);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// app.UseHttpsRedirection();

    app.UseMiddleware<ExceptionMiddleware>();
    app.MapControllers();

app.Run();

