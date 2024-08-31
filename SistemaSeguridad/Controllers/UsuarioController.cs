using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace SistemaSeguridad.Controllers
{
	public class UsuarioController: Controller
	{
		private readonly UserManager<UsuarioPrueba> userManager;
		private readonly IServicioUsuarios servicioUsuarios;
		private readonly SignInManager<UsuarioPrueba> signInManager;
		private readonly IRepositoryGenero repositoryGenero;
		private readonly IReposirorySucursal reposirorySucursal;
		private readonly RepositoryUsuarios repositoryUsuarios;

        public UsuarioController(UserManager<UsuarioPrueba> userManager, IServicioUsuarios servicioUsuarios,
			SignInManager<UsuarioPrueba> signInManager, IRepositoryGenero repositoryGenero, IReposirorySucursal reposirorySucursal, 
   			IWebHostEnvironment webHostEnvironment)
        {
			this.userManager = userManager;
			this.servicioUsuarios = servicioUsuarios;
			this.signInManager = signInManager;
			this.repositoryGenero = repositoryGenero;
			this.reposirorySucursal = reposirorySucursal;
   			this.webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
            return View();
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerGenero()
        {
            var modulo = await repositoryGenero.Obtener();
            return modulo.Select(x => new SelectListItem(x.Nombre, x.IdGenero.ToString()));
        }
		private async Task<IEnumerable<SelectListItem>> ObtenerSucursal()
		{
			var modulo = await reposirorySucursal.Obtener();
			return modulo.Select(x => new SelectListItem(x.Nombre, x.IdSucursal.ToString()));
		}
		/*
        public IActionResult Registro()
		{
			return View();
		}
		*/
		[HttpGet]
        public async Task<IActionResult> Registro()
        {
            var modelo = new RegistroCreacionViewModel();
            modelo.Genero = await ObtenerGenero();
			modelo.Sucursal = await ObtenerSucursal();
            return View(modelo);
        }

        [HttpPost]
		public async Task<IActionResult> Registro(RegistroCreacionViewModel modelo)
		{
			if (!ModelState.IsValid) 
			{
				return View(modelo);
			}

			string uniqueFileName = null;
			if (modelo.Fotografia != null)
			{
				string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
				uniqueFileName = Guid.NewGuid().ToString() + "_" + modelo.Fotografia.FileName;
				string filePath = Path.Combine(uploadsFolder, uniqueFileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await modelo.Fotografia.CopyToAsync(fileStream);
					Console.WriteLine("Path de la imagen: " + modelo.Fotografia);
				}
			}

            modelo.Genero = await ObtenerGenero();
			modelo.Sucursal = await ObtenerSucursal();

			var usuario = new UsuarioPrueba() { CorreoElectronico = modelo.CorreoElectronico, IdUsuario = modelo.IdUsuario,
												Nombre = modelo.Nombre, Apellido = modelo.Apellido, 
												FechaNacimiento = modelo.FechaNacimiento, TelefonoMovil = modelo.TelefonoMovil,
												UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId(), IdGenero = modelo.IdGenero,
												IdSucursal = modelo.IdSucursal, Fotografia = uniqueFileName};

			var resultado = await userManager.CreateAsync(usuario, password:modelo.Password);

			if (resultado.Succeeded)
			{
				//await signInManager.SignInAsync(usuario, isPersistent: false);
				return RedirectToAction("Index", "Usuario");
			}
			else 
			{
				foreach (var error in resultado.Errors) 
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return View(modelo);
		}


		[HttpPost]
		public async Task<IActionResult> Logout() 
		{
			await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
			return RedirectToAction("Index", "Home");
		}

        public async Task<IActionResult> Perfil()
        {
			var usuarioLogin = await userManager.GetUserAsync(User);

            if (usuarioLogin == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var userId = usuarioLogin.IdUsuario;
            var userName = usuarioLogin.Nombre;
	    var lastName = usuarioLogin.Apellido;
            var email = usuarioLogin.CorreoElectronico;
	    var password = usuarioLogin.Password;
     	    var Fotografia = usuarioLogin.Fotografia;

            var model = new UsuarioPrueba
            {
                IdUsuario = userId,
                Nombre = userName,
		 Apellido = lastName,
                CorreoElectronico = email,
		Password = password,
		Fotografia = Fotografia
            };

            return View(model);
        }
    }
}
