using API.Errors;
using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);
var app = builder.Build();




// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(policyBuilder => policyBuilder.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

//middleware
//do you have a valid token
app.UseAuthentication();
//what is your token valid for
app.UseAuthorization();

app.MapControllers();

app.Run();
