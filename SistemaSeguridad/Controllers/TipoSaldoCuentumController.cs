using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Servicios;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Controllers
{
    public class TipoSaldoCuentumController : Controller
    {
        private readonly IRepositoryTipoSaldoCuentum repositoryTipoSaldo;
        private readonly IServicioUsuarios servicioUsuarios;

        public TipoSaldoCuentumController(IRepositoryTipoSaldoCuentum repositoryTipoSaldo, IServicioUsuarios servicioUsuarios)
        {
            this.repositoryTipoSaldo = repositoryTipoSaldo;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var tipoSalddo = await repositoryTipoSaldo.ObtenerTodos();
            return View(tipoSalddo);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoSaldoCuentum tipoSaldoCuentum)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoSaldoCuentum);
            }

            tipoSaldoCuentum.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var existeTipoSaldo = await repositoryTipoSaldo.Existe(tipoSaldoCuentum.Nombre);

            if (existeTipoSaldo)
            {
                ModelState.AddModelError(nameof(tipoSaldoCuentum.Nombre), $"El nombre {tipoSaldoCuentum.Nombre} ya existe");
                return View(tipoSaldoCuentum);
            }
            await repositoryTipoSaldo.Crear(tipoSaldoCuentum);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int IdTipoSaldoCuenta)
        {
            var saldoCuenta = await repositoryTipoSaldo.ObtenerPorId(IdTipoSaldoCuenta);

            if (saldoCuenta is null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(saldoCuenta);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoSaldoCuentum tipoSaldoCuentum)
        {
            tipoSaldoCuentum.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var saldoExiste = await repositoryTipoSaldo.ObtenerPorId(tipoSaldoCuentum.IdTipoSaldoCuenta);

            if (saldoExiste is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryTipoSaldo.Actualizar(tipoSaldoCuentum);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int IdTipoSaldoCuenta)
        {
            var tipoSaldo = await repositoryTipoSaldo.ObtenerPorId(IdTipoSaldoCuenta);

            if (tipoSaldo is null)
            {
                return RedirectToAction("Index", "TipoSaldoCuentum");
            }
            return View(tipoSaldo);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarTipo(int IdTipoSaldoCuenta)
        {
            var saldo = await repositoryTipoSaldo.ObtenerPorId(IdTipoSaldoCuenta);
            if (saldo is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryTipoSaldo.Borrar(IdTipoSaldoCuenta);
            return RedirectToAction("Index");
        }
    }
}
