using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;

namespace SistemaSeguridad.Controllers
{
    public class OpcionController : Controller
    {
        private readonly IRepositoryOpcion repositoryOpcion;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositoryMenu repositoryMenu;

        public OpcionController(IRepositoryOpcion repositoryOpcion, IServicioUsuarios servicioUsuarios,
                                IRepositoryMenu repositoryMenu)
        {
            this.repositoryOpcion = repositoryOpcion;
            this.servicioUsuarios = servicioUsuarios;
            this.repositoryMenu = repositoryMenu;
        }

        public async Task<IActionResult> Index()
        {
            var opcion = await repositoryOpcion.Obtener();
            return View(opcion);
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerMenu()
        {
            var menu = await repositoryMenu.Obtener();
            return menu.Select(x => new SelectListItem(x.Nombre, x.IdMenu.ToString()));
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var modelo = new OpcionCreacionViewModel();
            modelo.Menu = await ObtenerMenu();
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(OpcionCreacionViewModel opcion)
        {
            if (!ModelState.IsValid)
            {
                return View(opcion);
            }

            opcion.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();
            opcion.Menu = await ObtenerMenu();

            var existeOpcion = await repositoryOpcion.Existe(opcion.Nombre);

            if (existeOpcion)
            {
                ModelState.AddModelError(nameof(opcion.Nombre), $"El nombre {opcion.Nombre} ya existe");
                return View(opcion);
            }

            await repositoryOpcion.Crear(opcion);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifarOpcion(string nombre)
        {
            var existeOpcion = await repositoryOpcion.Existe(nombre);
            if (existeOpcion)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        public async Task<IActionResult> Borrar(int idOpcion)
        {
            var opcion = await repositoryOpcion.ObtenerPorId(idOpcion);

            if (opcion is null)
            {
                return RedirectToAction("Index", "Opcion");
            }
            return View(opcion);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarOpcion(int idOpcion)
        {
            try
            {
                var opcion = await repositoryOpcion.ObtenerPorId(idOpcion);
                if (opcion is null)
                {
                    return RedirectToAction("Index", "Opcion");
                }
                await repositoryOpcion.Borrar(idOpcion);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex + "No se puede borrar este registro");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int idOpcion)
        {
            var opcion = await repositoryOpcion.ObtenerPorId(idOpcion);

            if (opcion is null)
            {
                return RedirectToAction("Index", "Opcion");
            }

            return View(opcion);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(Opcion opcion)
        {
            opcion.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var opcionExiste = await repositoryOpcion.ObtenerPorId(opcion.IdOpcion);

            if (opcionExiste is null)
            {
                return RedirectToAction("Index", "Opcion");
            }

            await repositoryOpcion.ActualizarGeneral(opcion);
            return RedirectToAction("Index");
        }

        // Método para exportar a CSV
        [HttpGet]
        public async Task<IActionResult> ExportarCsv()
        {
            var opciones = await repositoryOpcion.Obtener();
            var sb = new StringBuilder();

            // Cabecera del archivo CSV
            sb.AppendLine("IdOpcion,Nombre");

            // Contenido del CSV
            foreach (var opcion in opciones)
            {
                sb.AppendLine($"{opcion.IdOpcion},{opcion.Nombre}");
            }

            // Convertir el contenido a bytes
            var fileName = "Opciones.csv";
            var fileContent = Encoding.UTF8.GetBytes(sb.ToString());

            // Retornar el archivo CSV
            return File(fileContent, "text/csv", fileName);
        }
    }
}
