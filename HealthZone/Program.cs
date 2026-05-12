using HealthZone.Data;
using HealthZone.Models;
using HealthZone.Repositories;
using HealthZone.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddIdentity<Korisnik, IdentityRole>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.SeedRolesAsync(services);
}
app.Run();
