using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Servicios;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Controllers
{
    public class TipoMovimientoCxcController : Controller
    {
        private readonly IRepositoryTipoMovimientoCxc repositoryTipoMovimiento;
        private readonly IServicioUsuarios servicioUsuarios;

        public TipoMovimientoCxcController(IRepositoryTipoMovimientoCxc repositoryTipoMovimiento, IServicioUsuarios servicioUsuarios)
        {
            this.repositoryTipoMovimiento = repositoryTipoMovimiento;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var tipoMovimientos = await repositoryTipoMovimiento.ObtenerTodos();
            return View(tipoMovimientos);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoMovimientoCxc tipoMovimiento)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoMovimiento);
            }

            tipoMovimiento.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var tipoMovimientoExite = await repositoryTipoMovimiento.Existe(tipoMovimiento.Nombre);

            if (tipoMovimientoExite)
            {
                ModelState.AddModelError(nameof(tipoMovimiento.Nombre), $"El nombre {tipoMovimiento.Nombre} ya existe");
                return View(tipoMovimiento);
            }
            await repositoryTipoMovimiento.Crear(tipoMovimiento);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int IdTipoMovimientoCxc)
        {
            var movimientoCxc = await repositoryTipoMovimiento.ObtenerPorId(IdTipoMovimientoCxc);

            if (movimientoCxc is null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(movimientoCxc);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoMovimientoCxc tipoMovimiento)
        {
            tipoMovimiento.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var movimientoExiste = await repositoryTipoMovimiento.ObtenerPorId(tipoMovimiento.IdTipoMovimientoCxc);

            if (movimientoExiste is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryTipoMovimiento.Actualizar(tipoMovimiento);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int IdTipoMovimientoCxc)
        {
            var tipo = await repositoryTipoMovimiento.ObtenerPorId(IdTipoMovimientoCxc);

            if (tipo is null)
            {
                return RedirectToAction("Index", "TipoMovimientoCxc");
            }
            return View(tipo);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipo(int IdTipoMovimientoCxc)
        {
            var tipo = await repositoryTipoMovimiento.ObtenerPorId(IdTipoMovimientoCxc);
            if (tipo is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryTipoMovimiento.Borrar(IdTipoMovimientoCxc);
            return RedirectToAction("Index");
        }

    }
}
