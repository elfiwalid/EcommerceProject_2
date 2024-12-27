using EcommerceProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;




using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<IdentityOptions>(options =>
{
    // Configure les règles de mot de passe
    options.Password.RequireDigit = false; // Pas besoin de chiffres
    options.Password.RequiredLength = 6;   // Minimum 6 caractères
    options.Password.RequireNonAlphanumeric = false; // Pas besoin de caractères spéciaux
    options.Password.RequireUppercase = false; // Pas besoin de majuscules
    options.Password.RequireLowercase = false; // Pas besoin de minuscules
});


builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});


// Ajouter les services
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // Activer les sessions

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Activer l'authentification
app.UseAuthorization();  // Activer l'autorisation

app.UseSession(); // Utiliser les sessions



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Produits}/{action=Index}/{id?}");

app.Run();
