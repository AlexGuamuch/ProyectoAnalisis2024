using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;

namespace SistemaSeguridad.Controllers
{
    public class EmpresaController : Controller
    {
        private readonly IRepositoyEmpresa repositoyEmpresa;
        private readonly IServicioUsuarios servicioUsuarios;

        public EmpresaController(IRepositoyEmpresa repositoyEmpresa, IServicioUsuarios servicioUsuarios)
        {
            this.repositoyEmpresa = repositoyEmpresa;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var empresa = await repositoyEmpresa.Obtener();
            return View(empresa);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Empresa empresa)
        {
            if (!ModelState.IsValid)
            {
                return View(empresa);
            }

            empresa.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var existeEmpresa = await repositoyEmpresa.Existe(empresa.Nombre);

            if (existeEmpresa)
            {
                ModelState.AddModelError(nameof(empresa.Nombre), $"El nombre {empresa.Nombre} ya existe");
                return View(empresa);
            }
            await repositoyEmpresa.Crear(empresa);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifarEmpresa(string nombre)
        {
            var existeEmpresa = await repositoyEmpresa.Existe(nombre);
            if (existeEmpresa)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        public async Task<IActionResult> Borrar(int idEmpresa)
        {
            var empresa = await repositoyEmpresa.ObtenerPorId(idEmpresa);

            if (empresa is null)
            {
                return RedirectToAction("Index", "Empresa");
            }
            return View(empresa);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarEmpresa(int idEmpresa)
        {
            try
            {
                var empresa = await repositoyEmpresa.ObtenerPorId(idEmpresa);
                if (empresa is null)
                {
                    return RedirectToAction("Index", "Empresa");
                }
                await repositoyEmpresa.Borrar(idEmpresa);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex + "No se puede borrar este registro");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int idEmpresa)
        {
            var empresa = await repositoyEmpresa.ObtenerPorId(idEmpresa);

            if (empresa is null)
            {
                return RedirectToAction("Index", "Empresa");
            }

            return View(empresa);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(Empresa empresa)
        {
            empresa.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var empresaExiste = await repositoyEmpresa.ObtenerPorId(empresa.IdEmpresa);

            if (empresaExiste is null)
            {
                return RedirectToAction("Index", "Empresa");
            }

            await repositoyEmpresa.ActualizarGeneral(empresa);
            return RedirectToAction("Index");
        }

        // Metodo para exportar a CSV
        [HttpGet]
        public async Task<IActionResult> ExportarCSV()
        {
            var empresas = await repositoyEmpresa.Obtener();

            var builder = new StringBuilder();
            builder.AppendLine("IdEmpresa,Nombre,Direccion,NIT");

            foreach (var empresa in empresas)
            {
                builder.AppendLine($"{empresa.IdEmpresa},{empresa.Nombre},{empresa.Direccion},{empresa.Nit}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "Empresas.csv");
        }
    }
}
