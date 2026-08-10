using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    rateLimiterOptions.AddSlidingWindowLimiter("sliding", options =>
    {
        options.Window = TimeSpan.FromSeconds(30);
        options.PermitLimit = 10;
        options.SegmentsPerWindow = 3;
    });
});

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("GatewayExceptionHandler");

    logger.LogError(error, "Unhandled exception in gateway while processing {Method} {Path}",
        context.Request.Method, context.Request.Path);

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

    await context.Response.WriteAsJsonAsync(new
    {
        title = "Gateway Error",
        status = StatusCodes.Status500InternalServerError,
        detail = "An unexpected error occurred while processing the request."
    });
}));

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.Use(async (context, next) =>
{
    await next();

    if (!context.Response.HasStarted &&
        context.Response.StatusCode is StatusCodes.Status502BadGateway
            or StatusCodes.Status503ServiceUnavailable
            or StatusCodes.Status504GatewayTimeout)
    {
        var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("GatewayProxyErrorHandler");

        logger.LogError("Upstream service failure: {StatusCode} for {Method} {Path}",
            context.Response.StatusCode, context.Request.Method, context.Request.Path);

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            title = "Service Unavailable",
            status = context.Response.StatusCode,
            detail = "The upstream service is currently unavailable. Please try again later."
        });
    }
});

app.MapReverseProxy();


app.Run();
