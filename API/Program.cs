using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
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
using PaymentDetailApi.Infrastructure.EventHandlers.Payments;
using PaymentDetailApi.Infrastructure.EventHandlers.Products;
using PaymentDetailApi.Infrastructure.Persistence;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//rate limiter
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueLimit = 0;//reject
        options.PermitLimit = 2;//2 req in specified window(10)
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
    {
        options.Window = TimeSpan.FromSeconds(30);
        options.PermitLimit = 10;
        options.SegmentsPerWindow = 3;
    });
});

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

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DomainEventDispatchBehavior<,>));

builder.Services.AddScoped<DomainEventDispatcher>();
builder.Services.AddScoped<IDomainEventHandler<PaymentCreatedDomainEvent>, PaymentCreatedAuditHandler>();
builder.Services.AddScoped<IDomainEventHandler<PaymentDeletedDomainEvent>, PaymentDeletedAuditHandler>();
builder.Services.AddScoped<IDomainEventHandler<ProductStockAddedDomainEvent>, ProductStockAddedEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<OrderCreatedDomainEvent>, OrderCreatedEventHandler>();

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

app.UseCors(options =>
options.WithOrigins("http://localhost:4200")
.AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();

public partial class Program { }
