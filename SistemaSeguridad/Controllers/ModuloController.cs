using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;

namespace SistemaSeguridad.Controllers
{
    public class ModuloController : Controller
    {
        private readonly IRepositoryModulo repositoryModulo;
        private readonly IServicioUsuarios servicioUsuarios;

        public ModuloController(IRepositoryModulo repositoryModulo, IServicioUsuarios servicioUsuarios)
        {
            this.repositoryModulo = repositoryModulo;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var modulo = await repositoryModulo.Obtener();
            return View(modulo);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Modulo modulo)
        {
            if (!ModelState.IsValid)
            {
                return View(modulo);
            }

            modulo.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var existeModulo = await repositoryModulo.Existe(modulo.Nombre);

            if (existeModulo)
            {
                ModelState.AddModelError(nameof(modulo.Nombre), $"El nombre {modulo.Nombre} ya existe");
                return View(modulo);
            }
            await repositoryModulo.Crear(modulo);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifarModulo(string nombre)
        {
            var existeModulo = await repositoryModulo.Existe(nombre);
            if (existeModulo)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        public async Task<IActionResult> Borrar(int idModulo)
        {
            var modulo = await repositoryModulo.ObtenerPorId(idModulo);

            if (modulo is null)
            {
                return RedirectToAction("Index", "Modulo");
            }
            return View(modulo);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarModulo(int idModulo)
        {
            try
            {
                var modulo = await repositoryModulo.ObtenerPorId(idModulo);
                if (modulo is null)
                {
                    return RedirectToAction("Index", "Modulo");
                }
                await repositoryModulo.Borrar(idModulo);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex + "No se puede borrar este registro");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int idModulo)
        {
            var modulo = await repositoryModulo.ObtenerPorId(idModulo);

            if (modulo is null)
            {
                return RedirectToAction("Index", "Modulo");
            }

            return View(modulo);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(Modulo modulo)
        {
            modulo.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var moduloExiste = await repositoryModulo.ObtenerPorId(modulo.IdModulo);

            if (modulo is null)
            {
                return RedirectToAction("Index", "Modulo");
            }

            await repositoryModulo.ActualizarGeneral(modulo);
            return RedirectToAction("Index");
        }

        // Método para exportar a CSV
        [HttpGet]
        public async Task<IActionResult> ExportarCSV()
        {
            var modulos = await repositoryModulo.Obtener();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("IdModulo,Nombre"); // Encabezados del CSV

            foreach (var modulo in modulos)
            {
                csvBuilder.AppendLine($"{modulo.IdModulo},{modulo.Nombre}");
            }

            var csvData = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            var fileName = $"Modulos_{DateTime.Now:yyyyMMddHHmmss}.csv";

            return File(csvData, "text/csv", fileName);
        }
    }
}
