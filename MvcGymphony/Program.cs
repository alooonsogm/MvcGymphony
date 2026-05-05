using Microsoft.AspNetCore.Authentication.Cookies;
using MvcGymphony.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Add services to the container.

builder.Services.AddTransient<ServiceAuth>();
builder.Services.AddTransient<ServiceSesiones>();
builder.Services.AddTransient<ServiceReservaSesiones>();
builder.Services.AddTransient<ServiceUsuarios>();
builder.Services.AddTransient<ServiceHorarioUsuario>();
builder.Services.AddTransient<ServiceSalas>();
builder.Services.AddTransient<ServiceClases>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();

builder.Services.AddControllersWithViews(options => options.EnableEndpointRouting = false);

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

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.UseMvc(routes =>
{
    routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
