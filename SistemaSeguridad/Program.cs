using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SistemaSeguridad;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;

var builder = WebApplication.CreateBuilder(args);

var politicaUsuarioAutenticado = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
// Add services to the container.
builder.Services.AddControllersWithViews(opciones => { 
	opciones.Filters.Add(new AuthorizeFilter(politicaUsuarioAutenticado));
});
builder.Services.AddTransient<IRepositoryGenero, RepositoryGenero>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddTransient<IRepositoyEmpresa, RepositoryEmpresa>();
builder.Services.AddTransient<IRepositoryStatusUsuario, RepositoryStatusUsuario>();
builder.Services.AddTransient<IRepositoryRole, RepositoryRole>();
builder.Services.AddTransient<IRepositoryOpcion, RepositoryOpcion>();
builder.Services.AddTransient<IRepositoryModulo, RepositoryModulo>();
builder.Services.AddTransient<IRepositoryUsuarioRole, RepositoryUsuarioRole>();
builder.Services.AddTransient<IRepositoryBitacoraAcceso, RepositoryBitacoraAcceso>();
builder.Services.AddTransient<IRepositoryCuentaCorriente, RepositoryCuentaCorriente>();
builder.Services.AddScoped<IRepositoryDocumentoPersona, RepositoryDocumentoPersona>();
builder.Services.AddScoped<IRepositoryEstadoCivil, RepositoryEstadoCivil>();
builder.Services.AddScoped<IRepositoryMovimientoCuenta, RepositoryMovimientoCuenta>();
builder.Services.AddScoped<IRepositoryPersona, RepositoryPersona>();
builder.Services.AddScoped<IRepositorySaldoCuenta, RepositorySaldoCuenta>();
builder.Services.AddScoped<IRepositoryStatusCuentum, RepositoryStatusCuentum>();
builder.Services.AddScoped<IRepositoryTipoDocumento, RepositoryTipoDocumento>();
builder.Services.AddScoped<IRepositoryTipoMovimientoCxc, RepositoryTipoMovimientoCxc>();
builder.Services.AddScoped<IRepositoryTipoSaldoCuentum, RepositoryTipoSaldoCuentum>();
//builder.Services.AddScoped<IRepositoryFechaActiva, RepositoryFechaActiva>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IReposirorySucursal, RepositorySucursal>();
builder.Services.AddTransient<IRepositoryUsuarios, RepositoryUsuarios>();
builder.Services.AddTransient<IRepositoryMenu, RepositoryMenu>();
builder.Services.AddTransient<IUserStore<UsuarioPrueba>, UsuarioStore>();
builder.Services.AddScoped<IRepositoryUsuarioPregunta, UsuarioPreguntaRepository>();
builder.Services.AddTransient<SignInManager<UsuarioPrueba>>();
builder.Services.AddTransient<CierreService>();
builder.Services.AddIdentityCore<UsuarioPrueba>(opciones =>
{
	opciones.Password.RequireDigit = false; // Desactiva validaciones predeterminadas si es necesario
	opciones.Password.RequireLowercase = false;
	opciones.Password.RequireUppercase = false;
	opciones.Password.RequireNonAlphanumeric = false;
	opciones.Password.RequiredLength = 1;
}).AddPasswordValidator<CustomPasswordValidator>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
	options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
	options.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, opciones =>
{
	opciones.LoginPath = "/UsuarioLogin/Login";

});

builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
