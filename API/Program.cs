using API.Data;
using API.Entities;
using API.Errors;
using API.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    var appUserManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(appUserManager, roleManager);
}
catch (Exception e)
{
    var logger = services.GetService<ILogger>();
    logger.LogError(e, "An error occured during migration");
}
app.Run();
