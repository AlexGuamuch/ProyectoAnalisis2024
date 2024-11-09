using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;
using System.Threading.Tasks;

namespace SistemaSeguridad.Controllers
{
    public class MovimientoCuentaController : Controller
    {
        private readonly IRepositoryMovimientoCuenta repositoryMovimientoCuenta;
        private readonly IServicioUsuarios servicioUsuarios;

        public MovimientoCuentaController(
            IRepositoryMovimientoCuenta repositoryMovimientoCuenta,
            IServicioUsuarios servicioUsuarios)
        {
            this.repositoryMovimientoCuenta = repositoryMovimientoCuenta;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var movimientos = await repositoryMovimientoCuenta.Obtener();
            return View(movimientos);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(MovimientoCuentum movimientoCuenta)
        {
            if (!ModelState.IsValid)
            {
                return View(movimientoCuenta);
            }

            movimientoCuenta.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();
            movimientoCuenta.FechaCreacion = DateTime.Now;

            await repositoryMovimientoCuenta.Crear(movimientoCuenta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int idMovimientoCuenta)
        {
            var movimiento = await repositoryMovimientoCuenta.ObtenerPorId(idMovimientoCuenta);

            if (movimiento is null)
            {
                return RedirectToAction("Index");
            }

            return View(movimiento);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(MovimientoCuentum movimientoCuenta)
        {
            movimientoCuenta.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            movimientoCuenta.FechaModificacion = DateTime.Now;

            var movimientoExistente = await repositoryMovimientoCuenta.ObtenerPorId(movimientoCuenta.IdMovimientoCuenta);

            if (movimientoExistente is null)
            {
                return RedirectToAction("Index");
            }

            await repositoryMovimientoCuenta.ActualizarGeneral(movimientoCuenta);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int idMovimientoCuenta)
        {
            var movimiento = await repositoryMovimientoCuenta.ObtenerPorId(idMovimientoCuenta);

            if (movimiento is null)
            {
                return RedirectToAction("Index");
            }

            return View(movimiento);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarMovimiento(int idMovimientoCuenta)
        {
            var movimiento = await repositoryMovimientoCuenta.ObtenerPorId(idMovimientoCuenta);

            if (movimiento is null)
            {
                return RedirectToAction("Index");
            }

            await repositoryMovimientoCuenta.Borrar(idMovimientoCuenta);
            return RedirectToAction("Index");
        }

        // Método para exportar a CSV
        [HttpGet]
        public async Task<IActionResult> ExportarCSV()
        {
            var movimientos = await repositoryMovimientoCuenta.Obtener();
            var csv = new StringBuilder();
            csv.AppendLine("IdMovimientoCuenta,IdSaldoCuenta,IdTipoMovimientoCXC,FechaMovimiento,ValorMovimiento,ValorMovimientoPagado,Descripcion,FechaCreacion,UsuarioCreacion");

            foreach (var movimiento in movimientos)
            {
                csv.AppendLine($"{movimiento.IdMovimientoCuenta},{movimiento.IdSaldoCuenta}," +
                               $"{movimiento.IdTipoMovimientoCxc},{movimiento.FechaMovimiento:dd/MM/yyyy}," +
                               $"{movimiento.ValorMovimiento},{movimiento.ValorMovimientoPagado}," +
                               $"{movimiento.Descripcion},{movimiento.FechaCreacion:dd/MM/yyyy},{movimiento.UsuarioCreacion}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "movimientos_cuenta.csv");
        }
    }
}
