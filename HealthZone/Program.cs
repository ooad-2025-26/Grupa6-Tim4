using HealthZone.Areas.Identity.Data;
using HealthZone.Data;
using HealthZone.Models;
using HealthZone.Repositories;
using HealthZone.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<Korisnik>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// FIX: antiforgery cookie ne radi zbog HTTP/HTTPS mismatch
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    options.Cookie.SameSite = SameSiteMode.Lax;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddScoped<ITerminRepository, TerminRepository>();
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<INalazRepository, NalazRepository>();
builder.Services.AddScoped<IRecenzijaRepository, RecenzijaRepository>();
builder.Services.AddScoped<INotifikacijaRepository, NotifikacijaRepository>();
builder.Services.AddScoped<IKorisnikNaListiRepository, KorisnikNaListiRepository>();
builder.Services.AddScoped<IListaCekanjaRepository, ListaCekanjaRepository>();
builder.Services.AddScoped<IMedicinskaUslugaRepository, MedicinskaUslugaRepository>();
builder.Services.AddScoped<IZahtjevZaOpremuRepository, ZahtjevZaOpremuRepository>();

builder.Services.AddScoped<ITerminService, TerminService>();
builder.Services.AddScoped<IKorisnikService, KorisnikService>();
builder.Services.AddScoped<INalazService, NalazService>();
builder.Services.AddScoped<IRecenzijaService, RecenzijaService>();
builder.Services.AddScoped<INotifikacijaService, NotifikacijaService>();
builder.Services.AddScoped<IKorisnikNaListiService, KorisnikNaListiService>();
builder.Services.AddScoped<IListaCekanjaService, ListaCekanjaService>();
builder.Services.AddScoped<IMedicinskaUslugaService, MedicinskaUslugaService>();
builder.Services.AddScoped<IZahtjevZaOpremuService, ZahtjevZaOpremuService>();
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<PodsjetnikHostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// FIX: zakomentirano jer redirectuje HTTP→HTTPS i gubi antiforgery cookie
// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();


// Odjavi Windows machine account automatski
app.Use(async (context, next) =>
{
    if (context.User.Identity != null &&
        context.User.Identity.IsAuthenticated &&
        context.User.Identity.Name != null &&
        context.User.Identity.Name.Contains("\\") &&
        !context.Request.Path.StartsWithSegments("/Account/Login") &&
        !context.Request.Path.StartsWithSegments("/Account/Register") &&
        !context.Request.Path.StartsWithSegments("/Account/Logout") &&
        !context.Request.Path.StartsWithSegments("/Home") &&
        !context.Request.Path.StartsWithSegments("/MedicinskaUsluga") &&
        !context.Request.Path.StartsWithSegments("/css") &&
        !context.Request.Path.StartsWithSegments("/js") &&
        !context.Request.Path.StartsWithSegments("/lib") &&
        context.Request.Path.Value != "/")
    {
        await context.SignOutAsync();
        context.Response.Redirect("/Account/Login");
        return;
    }
    await next();
});

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.SeedRolesAsync(services);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Korisnik>>();
    // Kreiraj Administrator korisnika ako ne postoji
    string adminEmail = "admin@healthzone.com";
    string adminLozinka = "Admin123!";

    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new Korisnik
        {
            Ime = "Admin",
            Prezime = "HealthZone",
            Email = adminEmail,
            UserName = adminEmail
        };

        await userManager.CreateAsync(admin, adminLozinka);
        await userManager.AddToRoleAsync(admin, "Administrator");
    }
}
app.Run();