using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Servicios;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Controllers
{
    public class TipoSaldoCuentumController : Controller
    {
        private readonly IRepositoryTipoSaldoCuentum _repository;

        public TipoSaldoCuentumController(IRepositoryTipoSaldoCuentum repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var tiposSaldo = await _repository.ObtenerTodos();
            return View(tiposSaldo);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var tipoSaldo = await _repository.ObtenerPorId(id);
            if (tipoSaldo == null)
            {
                return NotFound();
            }
            return View(tipoSaldo);
        }
    }
}
