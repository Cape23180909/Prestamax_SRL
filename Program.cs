using BlazorBootstrap;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Prestamax_SRL.Components;
using Prestamax_SRL.Components.Account;
using Prestamax_SRL.Data;
using Prestamax_SRL.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var ConStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContextFactory<ApplicationDbContext>(o =>
    o.UseSqlServer(ConStr, options => options.CommandTimeout(20)) // Establece un tiempo de espera de 120 segundos
);

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<PrestamoService>();
builder.Services.AddScoped<CobroService>();
builder.Services.AddBlazorBootstrap();
builder.Services.AddSingleton<ToastService>();
builder.Services.AddSignalR();


var app = builder.Build();


// Configuraci�n del pipeline de solicitudes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error"); // Muestra una p�gina de error en producci�n.
    app.UseHsts(); // Habilita HSTS en producci�n.
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Protecci�n contra ataques CSRF.
app.UseAntiforgery();

// Mapea componentes de Razor y configura el renderizado interactivo del lado del servidor.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Asegura que se a�adan endpoints adicionales requeridos por Identity.
app.MapAdditionalIdentityEndpoints();

// Configura SignalR
app.MapHub<Hub>("/prestamoHub");
app.Run();