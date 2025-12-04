using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PeluqueriaCanina.Data;
using PeluqueriaCanina.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ContextoAcqua>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();


// Agregar HttpClientFactory para Mercado Pago
builder.Services.AddHttpClient();

builder.Services.AddHttpClient("VeterinariaApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7156/"); // <- ajustar
});

builder.Services.AddTransient<VeterinariaApiClient>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioActualService, UsuarioActualService>();



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

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ContextoAcqua>();
    DbInitializer.Initialize(context);
}

app.Run();
