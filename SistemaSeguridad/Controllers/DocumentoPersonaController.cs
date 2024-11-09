using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;
using System.Threading.Tasks;

namespace SistemaSeguridad.Controllers
{
    public class DocumentoPersonaController : Controller
    {
        private readonly IRepositoryDocumentoPersona repositoryDocumentoPersona;
        private readonly IServicioUsuarios servicioUsuarios;

        public DocumentoPersonaController(
            IRepositoryDocumentoPersona repositoryDocumentoPersona,
            IServicioUsuarios servicioUsuarios)
        {
            this.repositoryDocumentoPersona = repositoryDocumentoPersona;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var documentos = await repositoryDocumentoPersona.Obtener();
            return View(documentos);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(DocumentoPersona documentoPersona)
        {
            if (!ModelState.IsValid)
            {
                return View(documentoPersona);
            }

            documentoPersona.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var existeDocumento = await repositoryDocumentoPersona.Existe(
                documentoPersona.IdTipoDocumento,
                documentoPersona.IdPersona);

            if (existeDocumento)
            {
                ModelState.AddModelError(nameof(documentoPersona.NoDocumento),
                    $"El documento ya está registrado para esta persona.");
                return View(documentoPersona);
            }

            await repositoryDocumentoPersona.Crear(documentoPersona);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int idTipoDocumento, int idPersona)
        {
            var documento = await repositoryDocumentoPersona.ObtenerPorId(idTipoDocumento, idPersona);

            if (documento is null)
            {
                return RedirectToAction("Index");
            }

            return View(documento);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(DocumentoPersona documentoPersona)
        {
            documentoPersona.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var documentoExistente = await repositoryDocumentoPersona.ObtenerPorId(
                documentoPersona.IdTipoDocumento, documentoPersona.IdPersona);

            if (documentoExistente is null)
            {
                return RedirectToAction("Index");
            }

            await repositoryDocumentoPersona.ActualizarGeneral(documentoPersona);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Borrar(int idTipoDocumento, int idPersona)
        {
            var documento = await repositoryDocumentoPersona.ObtenerPorId(idTipoDocumento, idPersona);

            if (documento is null)
            {
                return RedirectToAction("Index");
            }

            return View(documento);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarDocumento(int idTipoDocumento, int idPersona)
        {
            var documento = await repositoryDocumentoPersona.ObtenerPorId(idTipoDocumento, idPersona);

            if (documento is null)
            {
                return RedirectToAction("Index");
            }

            await repositoryDocumentoPersona.Borrar(idTipoDocumento, idPersona);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerificarDocumento(int idTipoDocumento, int idPersona)
        {
            var existeDocumento = await repositoryDocumentoPersona.Existe(idTipoDocumento, idPersona);

            if (existeDocumento)
            {
                return Json($"El documento ya está registrado para esta persona.");
            }

            return Json(true);
        }

        // Método para exportar a CSV
        [HttpGet]
        public async Task<IActionResult> ExportarCSV()
        {
            var documentos = await repositoryDocumentoPersona.Obtener();
            var csv = new StringBuilder();
            csv.AppendLine("IdTipoDocumento,IdPersona,NoDocumento,FechaCreacion,UsuarioCreacion");

            foreach (var documento in documentos)
            {
                csv.AppendLine($"{documento.IdTipoDocumento},{documento.IdPersona}," +
                               $"{documento.NoDocumento},{documento.FechaCreacion},{documento.UsuarioCreacion}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "documentos_personas.csv");
        }
    }

}
