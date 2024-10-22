using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Servicios;

namespace SistemaSeguridad.Controllers
{
    public class EstadoCivilController : Controller
    {
        private readonly IRepositoryEstadoCivil _repository;

        public EstadoCivilController(IRepositoryEstadoCivil repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var estadosCiviles = await _repository.ObtenerTodos();
            return View(estadosCiviles);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var estadoCivil = await _repository.ObtenerPorId(id);
            if (estadoCivil == null) return NotFound();
            return View(estadoCivil);
        }
    }
}
