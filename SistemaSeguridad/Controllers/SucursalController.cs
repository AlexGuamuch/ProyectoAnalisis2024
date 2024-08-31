using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;

namespace SistemaSeguridad.Controllers
{
    public class SucursalController : Controller
    {
        private readonly IReposirorySucursal reposirorySucursal;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositoyEmpresa repositoyEmpresa;

        public SucursalController(IReposirorySucursal reposirorySucursal, IServicioUsuarios servicioUsuarios,
            IRepositoyEmpresa repositoyEmpresa)
        {
            this.reposirorySucursal = reposirorySucursal;
            this.servicioUsuarios = servicioUsuarios;
            this.repositoyEmpresa = repositoyEmpresa;
        }

        public async Task<IActionResult> Index()
        {
            var sucursal = await reposirorySucursal.Obtener();
            return View(sucursal);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var modelo = new SucursalCreacionViewModel();
            modelo.Empresa = await ObtenerEmpresas();
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(SucursalCreacionViewModel sucursal)
        {
            sucursal.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();
            sucursal.Empresa = await ObtenerEmpresas();
            await reposirorySucursal.Crear(sucursal);
            return RedirectToAction("Index");
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerEmpresas()
        {
            var empresa = await repositoyEmpresa.Obtener();
            return empresa.Select(x => new SelectListItem(x.Nombre, x.IdEmpresa.ToString()));
        }

        [HttpGet]
        public async Task<IActionResult> VerifarSucursal(string nombre)
        {
            var existeGenero = await reposirorySucursal.Existe(nombre);
            if (existeGenero)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        public async Task<IActionResult> Borrar(int idSucursal)
        {
            var sucursal = await reposirorySucursal.ObtenerPorId(idSucursal);

            if (sucursal is null)
            {
                return RedirectToAction("Index", "Sucursal");
            }
            return View(sucursal);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarSucursal(int idSucursal)
        {
            try
            {
                var sucursal = await reposirorySucursal.ObtenerPorId(idSucursal);
                if (sucursal is null)
                {
                    return RedirectToAction("Index", "Sucursal");
                }
                await reposirorySucursal.Borrar(idSucursal);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex + "No se puede borrar este registro");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int idSucursal)
        {
            var sucursal = await reposirorySucursal.ObtenerPorId(idSucursal);

            if (sucursal is null)
            {
                return RedirectToAction("Index", "Sucursal");
            }

            return View(sucursal);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(Sucursal sucursal)
        {
            sucursal.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var sucursalExiste = await reposirorySucursal.ObtenerPorId(sucursal.IdSucursal);

            if (sucursal is null)
            {
                return RedirectToAction("Index", "Sucursal");
            }

            await reposirorySucursal.ActualizarGeneral(sucursal);
            return RedirectToAction("Index");
        }

        // Método para exportar a CSV
        [HttpGet]
        public async Task<IActionResult> ExportarCSV()
        {
            var sucursales = await reposirorySucursal.Obtener();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("IdSucursal,Nombre, Direccion"); // Encabezados del CSV

            foreach (var sucursal in sucursales)
            {
                csvBuilder.AppendLine($"{sucursal.IdSucursal},{sucursal.Nombre},{sucursal.Direccion}");
            }

            var csvData = Encoding.UTF8.GetBytes(csvBuilder.ToString());
            var fileName = $"Sucursales_{DateTime.Now:yyyyMMddHHmmss}.csv";

            return File(csvData, "text/csv", fileName);
        }
    }
}
