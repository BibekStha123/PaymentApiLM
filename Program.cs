using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
