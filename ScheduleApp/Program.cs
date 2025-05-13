using ScheduleApp.Services;
using ScheduleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using ScheduleApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql("DefaultConnection"));

builder.Services.AddTransient<JSONDeserializer>();

builder.Services.AddHttpClient<JSONDeserializer>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ScheduleService>();

builder.Services.AddSession();


var app = builder.Build();
app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Dashboard}/{id?}",
    new { controller = "Admin" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Schedule}/{action=GetSchedule}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationContext>();
    var scheduleService = services.GetRequiredService<JSONDeserializer>();

    context.Database.EnsureCreated();
    AddingData.Initialize(context, scheduleService);
}


app.Run();
