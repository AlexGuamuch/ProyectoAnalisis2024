using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Servicios;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Controllers
{
    public class TipoMovimientoCxcController : Controller
    {
        private readonly IRepositoryTipoMovimientoCxc _repository;

        public TipoMovimientoCxcController(IRepositoryTipoMovimientoCxc repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var tiposMovimiento = await _repository.ObtenerTodos();
            return View(tiposMovimiento);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var tipoMovimiento = await _repository.ObtenerPorId(id);
            if (tipoMovimiento == null)
            {
                return NotFound();
            }
            return View(tipoMovimiento);
        }
    }
}
