using Microsoft.EntityFrameworkCore;
using Library.Application.Interfaces;
using Library.Application.Services;
using Library.Domain.Interfaces;
using Library.Infrastructure.Data;
using Library.Infrastructure.Repositories;
using Library.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Database Configuration ------
// Configures EntityCoreFramework to use SQL Server w/ connection string from appsettings.json
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories ------
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IBorrowRepository, BorrowRepository>();

// Services ------
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IBorrowService, BorrowService>();

// Caching —----- 
// Enables In-Memory caching for application
builder.Services.AddMemoryCache();

// API Setup ------
// Controller-based routing
builder.Services.AddControllers();

// launchSettings.json opens /swagger on launch,
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// HTTP Pipeline Configuration ------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

// Global Error Handling ------
app.UseMiddleware<ExceptionMiddleware>();

// Routes incoming requests to controller actions
app.MapControllers();
// Starts web application
app.Run();