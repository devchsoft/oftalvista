using Microsoft.EntityFrameworkCore;
using Oftalvista.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5000");

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularClient",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:4200", "https://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

builder.Services.AddDbContext<OftalvistaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OftalvistaDb")));

var app = builder.Build();

app.UseCors("AngularClient");
app.UseAuthorization();

app.MapGet(
    "/",
    () =>
        Results.Ok(
            new
            {
                name = "Oftalvista API",
                version = "1.0.0",
                baseUrl = "http://localhost:5000/api/v1",
            }
        )
);

app.MapControllers();

app.Run();
