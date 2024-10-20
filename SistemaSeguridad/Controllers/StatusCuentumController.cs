using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;

namespace SistemaSeguridad.Controllers
{
    public class StatusCuentumController : Controller
    {
        private readonly IRepositoryStatusCuentum _repository;

        public StatusCuentumController(IRepositoryStatusCuentum repository)
        {
            _repository = repository;
        }

        // GET: StatusCuentum
        public async Task<IActionResult> Index()
        {
            var statusCuentas = await _repository.ObtenerTodos();
            return View(statusCuentas);
        }

        // GET: StatusCuentum/5
        public async Task<IActionResult> Detalles(int id)
        {
            var statusCuenta = await _repository.ObtenerPorId(id);
            if (statusCuenta == null)
            {
                return NotFound();
            }
            return View(statusCuenta);
        }
    }
}
