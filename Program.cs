using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Fullcalendar.Data;
using Fullcalendar.Models.Entity;
using Fullcalendar.Service;
using Fullcalendar.Service.Impl;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Agregue las configuraciones para la conexion 
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL")));
builder.Services.AddIdentity<Usuario, IdentityRole>(cfg =>
    {
        cfg.User.RequireUniqueEmail = true;
        cfg.Password.RequireDigit = false;
        cfg.Password.RequiredUniqueChars = 0;
        cfg.Password.RequireLowercase = false;
        cfg.Password.RequireNonAlphanumeric = false;
        cfg.Password.RequireUppercase = false;

    }).AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();

builder.Services.AddNotyf(config =>
 {
     config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.TopCenter;
 });

builder.Services.ConfigureApplicationCookie(options => options.LoginPath = "/Login/IniciarSesion");

builder.Services.AddTransient<AdminDb>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ICalendarEventService, CalendarEventService>();
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

SeedData(app);
void SeedData(WebApplication app)
{
    IServiceScopeFactory scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (IServiceScope scope = scopedFactory.CreateScope())
    {
        AdminDb service = scope.ServiceProvider.GetService<AdminDb>();
        service.SeedAsync().Wait();
    }
}

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
app.UseAuthentication(); //agregue la autenticacion
app.UseAuthorization();
app.UseNotyf(); //agregue notify
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
