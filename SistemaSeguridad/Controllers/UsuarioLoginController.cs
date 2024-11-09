using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol.Core.Types;
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

		private string ObtenerSistemaOperativo(string userAgent)
		{
			if (userAgent.Contains("Windows NT 10.0"))
				return "Windows 10";
			else if (userAgent.Contains("Windows NT 6.3"))
				return "Windows 8.1";
			else if (userAgent.Contains("Windows NT 6.2"))
				return "Windows 8";
			else if (userAgent.Contains("Windows NT 6.1"))
				return "Windows 7";
			else if (userAgent.Contains("Mac OS X"))
				return "macOS";
			else if (userAgent.Contains("Linux"))
				return "Linux";
			else if (userAgent.Contains("Android"))
				return "Android";
			else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
				return "iOS";
			else
				return "Sistema operativo no identificado";
		}



		private string ObtenerTipoDispositivo(string userAgent)
		{
			userAgent = userAgent.ToLower();

			if (userAgent.Contains("mobile"))
				return "Móvil";
			else if (userAgent.Contains("tablet"))
				return "Tableta";
			else
				return "Escritorio";
		}

		private string ObtenerNavegador(string userAgent)
		{
			userAgent = userAgent.ToLower();

			if (userAgent.Contains("firefox"))
				return "Firefox";
			else if (userAgent.Contains("msie") || userAgent.Contains("trident"))
				return "Internet Explorer";
			else if (userAgent.Contains("edge"))
				return "Microsoft Edge";
			else if (userAgent.Contains("safari") && !userAgent.Contains("chrome"))
				return "Safari";
			else if (userAgent.Contains("chrome"))
				return "Google Chrome";
			else if (userAgent.Contains("opera") || userAgent.Contains("opr"))
				return "Opera";
			else
				return "Navegador desconocido";
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
				var userAgent = HttpContext.Request.Headers["User-Agent"];
				var uaParser = Parser.GetDefault();
				var clientInfo = uaParser.Parse(userAgent);
				var userId = usuario.IdUsuario;
				var HttpUserAgent = Request.Headers["User-Agent"];
				var sistemaOperativo = ObtenerSistemaOperativo(HttpUserAgent);
				var ipAddress = Request.HttpContext.Connection.RemoteIpAddress;
				var dispositivo = ObtenerTipoDispositivo(userAgent);
				var navegador = clientInfo.UA.Family;
				var bitacora = new BitacoraAcceso
				{
					IdUsuario = userId,
					HttpUserAgent = HttpUserAgent,
					DireccionIp = ipAddress.ToString(),
					Accion = "Login",
					SistemaOperativo = sistemaOperativo,
					Dispositivo = dispositivo,
					Browser = navegador
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

