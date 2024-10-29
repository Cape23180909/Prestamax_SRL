using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Prestamax_SRL.Components;
using Prestamax_SRL.Components.Account;
using Prestamax_SRL.Data;
using Prestamax_SRL.Services;

var builder = WebApplication.CreateBuilder(args);

// Agrega servicios al contenedor.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configuración de autenticación y servicios relacionados.
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Configuración de autenticación y cookies de identidad.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

// Configuración de la base de datos.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configuración de Identity con políticas de seguridad.
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Cookies solo a través de HTTPS.
    options.Cookie.SameSite = SameSiteMode.Strict; // Evita que se envíen en peticiones cross-site.
});

// Configuración de servicios personalizados.
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<PrestamoService>();
builder.Services.AddBlazorBootstrap();

var app = builder.Build();

// Configuración del pipeline de solicitudes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error"); // Muestra una página de error en producción.
    app.UseHsts(); // Habilita HSTS en producción.
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Protección contra ataques CSRF.
app.UseAntiforgery();

// Mapea componentes de Razor y configura el renderizado interactivo del lado del servidor.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Asegura que se añadan endpoints adicionales requeridos por Identity.
app.MapAdditionalIdentityEndpoints();

app.Run();