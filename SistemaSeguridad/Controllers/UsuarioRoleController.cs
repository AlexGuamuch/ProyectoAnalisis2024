using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;

namespace SistemaSeguridad.Controllers
{
    public class UsuarioRoleController: Controller
    {
        private readonly IRepositoryUsuarioRole repositoryUsuarioRole;
		private readonly IServicioUsuarios servicioUsuarios;
		private readonly IRepositoryRole repositoryRole;
		private readonly IRepositoryUsuarios repositoryUsuarios;

		public UsuarioRoleController(IRepositoryUsuarioRole repositoryUsuarioRole, IServicioUsuarios servicioUsuarios,
                IRepositoryRole repositoryRole, IRepositoryUsuarios repositoryUsuarios)
        {
            this.repositoryUsuarioRole = repositoryUsuarioRole;
			this.servicioUsuarios = servicioUsuarios;
			this.repositoryRole = repositoryRole;
			this.repositoryUsuarios = repositoryUsuarios;
		}

        public async Task<IActionResult> Index()
        {
            var usuarioRole = await repositoryUsuarioRole.Obtener();
            return View(usuarioRole);
        }

		public IActionResult Crear()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Crear(UsuarioRole usuarioRole)
		{
			if (!ModelState.IsValid)
			{
				return View(usuarioRole);
			}

			usuarioRole.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

			await repositoryUsuarioRole.Crear(usuarioRole);

			return RedirectToAction("Index");
		}
	}
}
