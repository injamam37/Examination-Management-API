using Scalar.AspNetCore;
using ExamApiDemo.Interfaces;
using ExamApiDemo.Services;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IExamResultService, ExamResultService>();

var connString = builder.Configuration.GetConnectionString("ExamDb")
    ?? throw new InvalidOperationException("Connection string 'ExamDb' not found.");

builder.Services.AddSingleton(connString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    message = "Exam API is running",
    service = "Examination Management API",
    status = "ok"
}));

app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "Examination Management API"
}));

app.MapGet("/health/db", async () =>
{
    try
    {
        await using var connection = new SqlConnection(connString);
        await connection.OpenAsync();

        return Results.Ok(new
        {
            status = "connected",
            database = "SQL Server"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Database connection failed",
            detail: "The API could not connect to the configured SQL Server database.",
            statusCode: StatusCodes.Status503ServiceUnavailable,
            extensions: new Dictionary<string, object?>
            {
                ["error"] = ex.Message
            });
    }
});

app.Run();

