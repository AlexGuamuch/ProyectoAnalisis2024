using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;

namespace SistemaSeguridad.Controllers
{
    public class StatusCuentumController : Controller
    {
        private readonly IRepositoryStatusCuentum repositoryStatusCuentum;
        private readonly IServicioUsuarios servicioUsuarios;

        public StatusCuentumController(IRepositoryStatusCuentum repositoryStatusCuentum, IServicioUsuarios servicioUsuarios)
        {
            this.repositoryStatusCuentum = repositoryStatusCuentum;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var estadoCiviel = await repositoryStatusCuentum.ObtenerTodos();
            return View(estadoCiviel);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(StatusCuentum statusCuentum)
        {
            if (!ModelState.IsValid)
            {
                return View(statusCuentum);
            }

            statusCuentum.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var existeStatusCuenta = await repositoryStatusCuentum.Existe(statusCuentum.Nombre);

            if (existeStatusCuenta)
            {
                ModelState.AddModelError(nameof(statusCuentum.Nombre), $"El nombre {statusCuentum.Nombre} ya existe");
                return View(statusCuentum);
            }
            await repositoryStatusCuentum.Crear(statusCuentum);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int IdStatusCuenta)
        {
            var statustipo = await repositoryStatusCuentum.ObtenerPorId(IdStatusCuenta);

            if (statustipo is null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(statustipo);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(StatusCuentum statusCuentum)
        {
            statusCuentum.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var statusExiste = await repositoryStatusCuentum.ObtenerPorId(statusCuentum.IdStatusCuenta);

            if (statusExiste is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryStatusCuentum.Actualizar(statusCuentum);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int IdStatusCuenta)
        {
            var statusCuentum = await repositoryStatusCuentum.ObtenerPorId(IdStatusCuenta);

            if (statusCuentum is null)
            {
                return RedirectToAction("Index", "StatusCuentum");
            }
            return View(statusCuentum);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarStatus(int IdStatusCuenta)
        {
            var status = await repositoryStatusCuentum.ObtenerPorId(IdStatusCuenta);
            if (status is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryStatusCuentum.Borrar(IdStatusCuenta);
            return RedirectToAction("Index");
        }

    }
}
