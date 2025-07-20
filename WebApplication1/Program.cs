using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuraci�n de sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiraci�n de la sesi�n
    options.Cookie.HttpOnly = true; // La cookie solo es accesible por el servidor
    options.Cookie.IsEssential = true; // La cookie es esencial para el funcionamiento b�sico
});

// Lee la cadena de conexi�n desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Registra el ApplicationDbContext con EF Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<EmailService>();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccesoDenegado";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Cliente", policy => policy.RequireClaim("Role", "1"));
    options.AddPolicy("Chef", policy => policy.RequireClaim("Role", "2"));
    options.AddPolicy("Mesero", policy => policy.RequireClaim("Role", "3"));
    options.AddPolicy("Gerente", policy => policy.RequireClaim("Role", "4"));
    options.AddPolicy("Dios del caos", policy => policy.RequireClaim("Role", "5"));
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Agregar HSTS en producci�n
}

app.UseStaticFiles();

app.UseRouting();

// Agregar middleware de autenticaci�n y autorizaci�n
app.UseAuthentication(); // Necesario para el sistema de autenticaci�n
app.UseAuthorization();

// Habilitar el uso de sesiones
app.UseSession();

app.MapControllerRoute(
    name: "default",
    // Cambiar la ruta por defecto al login de cliente
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();