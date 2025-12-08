using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar DbContext
builder.Services.AddDbContext<ContextoAcqua>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicios MVC
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddSession();

// Autenticación con cookies personalizada
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Home/AccesoDenegado";
    });

// Servicio de envío de emails
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
//Servicios para auditoria.
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();


// HttpClientFactory
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("VeterinariaApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7156/"); // <- ajustar según tu API
});
builder.Services.AddTransient<VeterinariaApiClient>();

// Usuario actual
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<AdminEntryStrategy>();
builder.Services.AddTransient<ClienteEntryStrategy>();
builder.Services.AddTransient<PeluqueroEntryStrategy>();
builder.Services.AddScoped<AuditoriaService>();
builder.Services.AddScoped<IUsuarioActualService, UsuarioActualService>();



var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseMiddleware<UsuarioActualMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// Inicializar base de datos
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ContextoAcqua>();
    DbInitializer.Initialize(context);
}

app.Run();
