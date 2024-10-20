using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;

namespace SistemaSeguridad.Controllers
{
    public class TipoDocumentoController : Controller
    {
        private readonly IRepositoryTipoDocumento repositoryTipoDocumento;

        public TipoDocumentoController(IRepositoryTipoDocumento repositoryTipoDocumento)
        {
            this.repositoryTipoDocumento = repositoryTipoDocumento;
        }

        public async Task<IActionResult> Index()
        {
            var tiposDocumento = await repositoryTipoDocumento.ObtenerTodos();
            return View(tiposDocumento);
        }

        public async Task<IActionResult> Detalles(int idTipoDocumento)
        {
            var tipoDocumento = await repositoryTipoDocumento.ObtenerPorId(idTipoDocumento);
            if (tipoDocumento is null)
            {
                return NotFound();
            }
            return View(tipoDocumento);
        }
    }
}
