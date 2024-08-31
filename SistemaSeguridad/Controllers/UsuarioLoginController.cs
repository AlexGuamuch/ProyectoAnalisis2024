using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Data;
using UAParser;


namespace SistemaSeguridad.Controllers
{
	public class UsuarioLoginController : Controller
	{
		private readonly UserManager<UsuarioPrueba> userManager;
		private readonly IServicioUsuarios servicioUsuarios;
		private readonly SignInManager<UsuarioPrueba> signInManager;
		private readonly IRepositoryBitacoraAcceso repositoryBitacoraAcceso;

		public UsuarioLoginController(UserManager<UsuarioPrueba> userManager, IServicioUsuarios servicioUsuarios,
			SignInManager<UsuarioPrueba> signInManager, IRepositoryBitacoraAcceso repositoryBitacoraAcceso)
		{

			this.userManager = userManager;
			this.servicioUsuarios = servicioUsuarios;
			this.signInManager = signInManager;
			this.repositoryBitacoraAcceso = repositoryBitacoraAcceso;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginViewModel modelo)
		{
			if (!ModelState.IsValid)
			{
				return View(modelo);
			}

			var usuario = await userManager.FindByNameAsync(modelo.IdUsuario);
			if (usuario == null)
			{
				ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto");
				return View(modelo);
			}

			// Comprobar si la cuenta está bloqueada
			if (usuario.FechaDesbloqueo.HasValue && usuario.FechaDesbloqueo.Value > DateTime.Now)
			{
				ModelState.AddModelError(string.Empty, "Su cuenta está bloqueada temporalmente. Inténtelo de nuevo en 5 minutos.");
				return View(modelo);
			}

			var resultado = await signInManager.PasswordSignInAsync(modelo.IdUsuario, password: modelo.Password,
																	modelo.Recuerdame, lockoutOnFailure: false);

			if (resultado.Succeeded)
			{ 
   				//bitacora acceso
				var userId = usuario.IdUsuario;
				var HttpUserAgent = Request.Headers["User-Agent"];
    				var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
				var bitacora = new BitacoraAcceso
				{
					IdUsuario = userId,
					HttpUserAgent = HttpUserAgent,
     					DireccionIp = ipAddress
				};
				await repositoryBitacoraAcceso.Bitacora(bitacora);
				// Reiniciar contadores y fecha de desbloqueo
				usuario.IntentosDeAcceso = 0;
				usuario.FechaDesbloqueo = null;
				await userManager.UpdateAsync(usuario);

				return RedirectToAction("Index", "Home");
			}
			else
			{
				// Incrementar el contador de intentos fallidos
				usuario.IntentosDeAcceso++;
				if (usuario.IntentosDeAcceso >= 5)
				{
					usuario.FechaDesbloqueo = DateTime.Now;
					await userManager.UpdateAsync(usuario);
					ModelState.AddModelError(string.Empty, "Su cuenta ha sido bloqueada temporalmente. Inténtelo de nuevo en 5 minutos.");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Nombre de usuario o password incorrecto");
				}
				await userManager.UpdateAsync(usuario);
				return View(modelo);
			}
		}


	}
}

