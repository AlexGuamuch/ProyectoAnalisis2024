using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;
using System.Threading.Tasks;

namespace SistemaSeguridad.Controllers
{
    public class GeneroController : Controller
    {
        private readonly IRepositoryGenero repositoryGenero;
        private readonly IServicioUsuarios servicioUsuarios;

        public GeneroController(IRepositoryGenero repositoryGenero,
            IServicioUsuarios servicioUsuarios)
        {
            this.repositoryGenero = repositoryGenero;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var genero = await repositoryGenero.Obtener();
            return View(genero);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Genero genero)
        {
            if (!ModelState.IsValid)
            {
                return View(genero);
            }

            genero.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var existeGenero = await repositoryGenero.Existe(genero.Nombre);

            if (existeGenero)
            {
                ModelState.AddModelError(nameof(genero.Nombre), $"El nombre {genero.Nombre} ya existe");
                return View(genero);
            }
            await repositoryGenero.Crear(genero);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int IdGenero)
        {
            var generotipo = await repositoryGenero.ObtenerPorId(IdGenero);

            if (generotipo is null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(generotipo);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(Genero genero)
        {
            genero.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var tipocuentaExiste = await repositoryGenero.ObtenerPorId(genero.IdGenero);

            if (tipocuentaExiste is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryGenero.Actualizar(genero);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int idGenero)
        {
            var genero = await repositoryGenero.ObtenerPorId(idGenero);

            if (genero is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(genero);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarGenero(int idGenero)
        {
            var genero = await repositoryGenero.ObtenerPorId(idGenero);
            if (genero is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryGenero.Borrar(idGenero);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifarGenero(string nombre)
        {
            var existeGenero = await repositoryGenero.Existe(nombre);
            if (existeGenero)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        // funcion para exportar a CSV
        [HttpGet]

        public async Task<IActionResult> ExportarCSV()
        {
            var generos = await repositoryGenero.Obtener();
            var csv = new StringBuilder();
            csv.AppendLine("Nombre, IdGenero"); // Encabezado de la columna

            foreach (var genero in generos)
            {
                csv.AppendLine($"{genero.IdGenero},{genero.Nombre}"); // Datos del CSV
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "generos.csv");
        }
    }
}
