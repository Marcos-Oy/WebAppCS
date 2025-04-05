using WebAppCS.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Agregar servicio de la base de datos
builder.Services.AddSingleton<Database>();

// Configuración de sesiones
builder.Services.AddDistributedMemoryCache();  // Usar una memoria en caché distribuida para sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // El tiempo de vida de la sesión
    options.Cookie.HttpOnly = true; // Asegura que el cliente no puede acceder a la cookie
    options.Cookie.IsEssential = true; // Necesario para cumplir con las leyes de privacidad
});

// Agregar servicios para controladores y vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 Habilitar WebSockets
app.UseWebSockets();

// Configurar el pipeline de solicitudes HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 🔹 Agregar middleware de sesión
app.UseSession();  // Este middleware es necesario para que la sesión funcione

app.UseAuthorization();

// Configurar las rutas para los controladores
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
