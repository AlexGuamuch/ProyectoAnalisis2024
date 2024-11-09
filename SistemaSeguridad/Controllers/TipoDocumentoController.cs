using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;

namespace SistemaSeguridad.Controllers
{
    public class TipoDocumentoController : Controller
    {
        private readonly IRepositoryTipoDocumento repositoryTipoDocumento;
        private readonly IServicioUsuarios servicioUsuarios;

        public TipoDocumentoController(IRepositoryTipoDocumento repositoryTipoDocumento, IServicioUsuarios servicioUsuarios)
        {
            this.repositoryTipoDocumento = repositoryTipoDocumento;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var tipoDocumento = await repositoryTipoDocumento.ObtenerTodos();
            return View(tipoDocumento);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoDocumento tipoDocumento)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoDocumento);
            }

            tipoDocumento.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var existeTipoDocumento = await repositoryTipoDocumento.Existe(tipoDocumento.Nombre);

            if (existeTipoDocumento)
            {
                ModelState.AddModelError(nameof(tipoDocumento.Nombre), $"El nombre {tipoDocumento.Nombre} ya existe");
                return View(tipoDocumento);
            }
            await repositoryTipoDocumento.Crear(tipoDocumento);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int IdTipoDocumento)
        {
            var documentotipo = await repositoryTipoDocumento.ObtenerPorId(IdTipoDocumento);

            if (documentotipo is null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(documentotipo);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoDocumento tipoDocumento)
        {
            tipoDocumento.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var documentoExiste = await repositoryTipoDocumento.ObtenerPorId(tipoDocumento.IdTipoDocumento);

            if (documentoExiste is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryTipoDocumento.Actualizar(tipoDocumento);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int IdTipoDocumento)
        {
            var tipoDocumento = await repositoryTipoDocumento.ObtenerPorId(IdTipoDocumento);

            if (tipoDocumento is null)
            {
                return RedirectToAction("Index", "TipoDocumento");
            }
            return View(tipoDocumento);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarDocumento(int IdTipoDocumento)
        {
            var documento = await repositoryTipoDocumento.ObtenerPorId(IdTipoDocumento);
            if (documento is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryTipoDocumento.Borrar(IdTipoDocumento);
            return RedirectToAction("Index");
        }
    }
}
