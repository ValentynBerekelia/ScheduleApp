using ScheduleApp.Services;
using ScheduleApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<JSONDeserializer>();


// Add services to the container.
builder.Services.AddHttpClient<JSONDeserializer>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<RootRepository>();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Schedule}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationContext>();
    var scheduleService = services.GetRequiredService<JSONDeserializer>();

    context.Database.EnsureCreated();
    AddingData.Initialize(context, scheduleService);
}


app.Run();
