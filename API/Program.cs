using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PaymentDetailApi.Application.Common.Behaviors;
using PaymentDetailApi.Application.Common.Interfaces;
using PaymentDetailApi.Domain.Catalog.Events;
using PaymentDetailApi.Domain.Common;
using PaymentDetailApi.Domain.Orders.Events;
using PaymentDetailApi.Domain.Payment.Events;
using PaymentDetailApi.Infrastructure.Auth;
using PaymentDetailApi.Infrastructure.DomainEvents;
using PaymentDetailApi.Infrastructure.EventHandlers.Orders;
using PaymentDetailApi.Infrastructure.Notification;
using PaymentDetailApi.Infrastructure.EventHandlers.Payments;
using PaymentDetailApi.Infrastructure.EventHandlers.Products;
using PaymentDetailApi.Infrastructure.Persistence;
using System.Text;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHttpClient("LMStudio", client =>
{
    client.BaseAddress = new Uri("http://localhost:1234");
    client.Timeout = TimeSpan.FromMinutes(5);
    var apiKey = builder.Configuration["LMStudio:ApiKey"] ?? "lm-studio";
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
});

builder.Services.AddDbContext<PaymentDetailsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentDetailContext")
    ?? throw new InvalidOperationException("Connection string 'PaymentDetailContext' not found.")));

builder.Services.AddMediatR(cfg =>
     cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DomainEventDispatchBehavior<,>));

builder.Services.AddScoped<DomainEventDispatcher>();
builder.Services.AddScoped<IDomainEventHandler<PaymentCreatedDomainEvent>, PaymentCreatedAuditHandler>();
builder.Services.AddScoped<IDomainEventHandler<PaymentDeletedDomainEvent>, PaymentDeletedAuditHandler>();
builder.Services.AddScoped<IDomainEventHandler<ProductStockAddedDomainEvent>, ProductStockAddedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<ProductStockRemovedDomainEvent>, ProductStockRemovedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<OrderCreatedDomainEvent>, OrderCreatedEventHandler>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/health", () => Results.Ok("Healthy"));

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("GlobalExceptionHandler");

    context.Response.ContentType = "application/json";

    switch (error)
    {
        case ValidationException validationException:
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Validation Failed",
                status = StatusCodes.Status400BadRequest,
                errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
            break;

        case KeyNotFoundException notFoundException:
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Not Found",
                status = StatusCodes.Status404NotFound,
                detail = notFoundException.Message
            });
            break;

        case UnauthorizedAccessException unauthorizedException:
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Unauthorized",
                status = StatusCodes.Status401Unauthorized,
                detail = unauthorizedException.Message
            });
            break;

        case InvalidOperationException invalidOperationException:
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Conflict",
                status = StatusCodes.Status409Conflict,
                detail = invalidOperationException.Message
            });
            break;

        case ArgumentException argumentException:
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Bad Request",
                status = StatusCodes.Status400BadRequest,
                detail = argumentException.Message
            });
            break;

        default:
            logger.LogError(error, "Unhandled exception occurred while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                title = "An unexpected error occurred",
                status = StatusCodes.Status500InternalServerError
            });
            break;
    }
}));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
