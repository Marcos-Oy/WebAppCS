using WebAppCS.Data;
using WebAppCS.Filters;
using WebAppCS.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Servicios esenciales
builder.Services.AddSingleton<Database>();
builder.Services.AddHttpContextAccessor();

// Configuración de sesión
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registrar controladores
builder.Services.AddScoped<AccountController>();

// Configurar filtros
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<AuthFilter>();
});

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();