using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;

namespace SistemaSeguridad.Controllers
{
    public class StatusUsuarioController : Controller
    {
        private readonly IRepositoryStatusUsuario repositoryStatusUsuario;
        private readonly IServicioUsuarios servicioUsuarios;

        public StatusUsuarioController(IRepositoryStatusUsuario repositoryStatusUsuario, IServicioUsuarios servicioUsuarios)
        {
            this.repositoryStatusUsuario = repositoryStatusUsuario;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var statusUsuario = await repositoryStatusUsuario.Obtener();
            return View(statusUsuario);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(StatusUsuario statusUsuario)
        {
            if (!ModelState.IsValid)
            {
                return View(statusUsuario);
            }

            statusUsuario.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var existeUsuario = await repositoryStatusUsuario.Existe(statusUsuario.Nombre);

            if (existeUsuario)
            {
                ModelState.AddModelError(nameof(statusUsuario.Nombre), $"El nombre {statusUsuario.Nombre} ya existe");
                return View(statusUsuario);
            }
            await repositoryStatusUsuario.Crear(statusUsuario);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifarStatusUsusario(string nombre)
        {
            var existeStatusUsuario = await repositoryStatusUsuario.Existe(nombre);
            if (existeStatusUsuario)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        public async Task<IActionResult> Borrar(int idStatusUsuario)
        {
            var statusUsuario = await repositoryStatusUsuario.ObtenerPorId(idStatusUsuario);

            if (statusUsuario is null)
            {
                return RedirectToAction("Index", "StatusUsuario");
            }
            return View(statusUsuario);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarStatusUsuario(int idStatusUsuario)
        {
            try
            {
                var statusUsuario = await repositoryStatusUsuario.ObtenerPorId(idStatusUsuario);
                if (statusUsuario is null)
                {
                    return RedirectToAction("Index", "StatusUsuario");
                }
                await repositoryStatusUsuario.Borrar(idStatusUsuario);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex + "No se puede borrar este registro");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int idStatusUsuario)
        {
            var statusUsuario = await repositoryStatusUsuario.ObtenerPorId(idStatusUsuario);

            if (statusUsuario is null)
            {
                return RedirectToAction("Index", "StatusUsuario");
            }

            return View(statusUsuario);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(StatusUsuario statusUsuario)
        {
            statusUsuario.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var estatusExiste = await repositoryStatusUsuario.ObtenerPorId(statusUsuario.IdStatusUsuario);

            if (estatusExiste is null)
            {
                return RedirectToAction("Index", "StatusUsuario");
            }

            await repositoryStatusUsuario.ActualizarGeneral(statusUsuario);
            return RedirectToAction("Index");
        }

        // Método para exportar a CSV
        [HttpGet]
        public async Task<IActionResult> ExportarCsv()
        {
            var statusUsuarios = await repositoryStatusUsuario.Obtener();
            var sb = new StringBuilder();

            // Cabecera del archivo CSV
            sb.AppendLine("IdStatusUsuario,Nombre");

            // Contenido del CSV
            foreach (var statusUsuario in statusUsuarios)
            {
                sb.AppendLine($"{statusUsuario.IdStatusUsuario},{statusUsuario.Nombre}");
            }

            // Convertir el contenido a bytes
            var fileName = "StatusUsuarios.csv";
            var fileContent = Encoding.UTF8.GetBytes(sb.ToString());

            // Retornar el archivo CSV
            return File(fileContent, "text/csv", fileName);
        }
    }
}
