using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;

namespace SistemaSeguridad.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRepositoryRole repositoryRole;
        private readonly IServicioUsuarios servicioUsuarios;

        public RoleController(IRepositoryRole repositoryRole, IServicioUsuarios servicioUsuarios)
        {
            this.repositoryRole = repositoryRole;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var role = await repositoryRole.Obtener();
            return View(role);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Role role)
        {
            if (!ModelState.IsValid)
            {
                return View(role);
            }

            role.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var existeRole = await repositoryRole.Existe(role.Nombre);

            if (existeRole)
            {
                ModelState.AddModelError(nameof(role.Nombre), $"El nombre {role.Nombre} ya existe");
                return View(role);
            }
            await repositoryRole.Crear(role);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifarRole(string nombre)
        {
            var existeRole = await repositoryRole.Existe(nombre);
            if (existeRole)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        public async Task<IActionResult> Borrar(int idRole)
        {
            var role = await repositoryRole.ObtenerPorId(idRole);

            if (role is null)
            {
                return RedirectToAction("Index", "Role");
            }
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarRole(int idRole)
        {
            try
            {
                var role = await repositoryRole.ObtenerPorId(idRole);
                if (role is null)
                {
                    return RedirectToAction("Index", "Role");
                }
                await repositoryRole.Borrar(idRole);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex + "No se puede borrar este registro");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int idRole)
        {
            var role = await repositoryRole.ObtenerPorId(idRole);

            if (role is null)
            {
                return RedirectToAction("Index", "Role");
            }

            return View(role);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(Role role)
        {
            role.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var roleExiste = await repositoryRole.ObtenerPorId(role.IdRole);

            if (roleExiste is null)
            {
                return RedirectToAction("Index", "Role");
            }

            await repositoryRole.ActualizarGeneral(role);
            return RedirectToAction("Index");
        }

        // Método para exportar a CSV
        [HttpGet]
        public async Task<IActionResult> ExportarCsv()
        {
            var roles = await repositoryRole.Obtener();
            var sb = new StringBuilder();

            // Cabecera del archivo CSV
            sb.AppendLine("IdRole,Nombre");

            // Contenido del CSV
            foreach (var role in roles)
            {
                sb.AppendLine($"{role.IdRole},{role.Nombre}");
            }

            // Convertir el contenido a bytes
            var fileName = "Roles.csv";
            var fileContent = Encoding.UTF8.GetBytes(sb.ToString());

            // Retornar el archivo CSV
            return File(fileContent, "text/csv", fileName);
        }
    }
}
