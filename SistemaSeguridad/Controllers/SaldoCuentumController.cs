using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;

namespace SistemaSeguridad.Controllers
{
    public class SaldoCuentumController : Controller
    {
        private readonly IRepositorySaldoCuenta _repository;

        public SaldoCuentumController(IRepositorySaldoCuenta repository)
        {
            _repository = repository;
        }

        // Renderiza la vista inicial con un formulario de búsqueda y espacio para resultados
        [HttpGet]
        public IActionResult Index()
        {
            return View(Enumerable.Empty<SaldoCuentum>()); // Lista vacía en la carga inicial
        }

        // Maneja la búsqueda según el tipo seleccionado
        [HttpPost]
        public async Task<IActionResult> Index(string tipoBusqueda, string valorBusqueda)
        {
            // Validar el valor de búsqueda
            if (string.IsNullOrWhiteSpace(valorBusqueda))
            {
                ModelState.AddModelError(string.Empty, "Por favor, ingrese un valor de búsqueda.");
                return View(Enumerable.Empty<SaldoCuentum>());
            }

            // Buscar según el tipo de búsqueda
            IEnumerable<SaldoCuentum> resultados;

            switch (tipoBusqueda)
            {
                case "Nombre":
                    resultados = await _repository.BuscarPorNombre(valorBusqueda);
                    break;
                case "DPI":
                    resultados = await _repository.BuscarPorDpi(valorBusqueda);
                    break;
                case "NumeroCuenta":
                    resultados = await _repository.BuscarPorNumeroCuenta(valorBusqueda);
                    break;
                default:
                    ModelState.AddModelError(string.Empty, "Tipo de búsqueda no válido.");
                    return View(Enumerable.Empty<SaldoCuentum>());
            }

            // Retorna los resultados a la misma vista
            return View(resultados);
        }
    }
}
