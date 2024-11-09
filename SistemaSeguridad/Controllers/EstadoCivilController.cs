using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;

namespace SistemaSeguridad.Controllers
{
    public class EstadoCivilController : Controller
    {
        private readonly IRepositoryEstadoCivil repositoryEstadoCivil;
        private readonly IServicioUsuarios servicioUsuarios;

        public EstadoCivilController(IRepositoryEstadoCivil repositoryEstadoCivil, IServicioUsuarios servicioUsuarios)
        {
            this.repositoryEstadoCivil = repositoryEstadoCivil;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var estadoCiviel = await repositoryEstadoCivil.ObtenerTodos();
            return View(estadoCiviel);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(EstadoCivil estadoCivil)
        {
            if (!ModelState.IsValid)
            {
                return View(estadoCivil);
            }

            estadoCivil.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();

            var existeEstadoCivil = await repositoryEstadoCivil.Existe(estadoCivil.Nombre);

            if (existeEstadoCivil)
            {
                ModelState.AddModelError(nameof(estadoCivil.Nombre), $"El nombre {estadoCivil.Nombre} ya existe");
                return View(estadoCivil);
            }
            await repositoryEstadoCivil.Crear(estadoCivil);

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult> Editar(int IdEstadoCivil)
        {
            var estadotipo = await repositoryEstadoCivil.ObtenerPorId(IdEstadoCivil);

            if (estadotipo is null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(estadotipo);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(EstadoCivil estadoCivil)
        {
            estadoCivil.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var estadoExiste = await repositoryEstadoCivil.ObtenerPorId(estadoCivil.IdEstadoCivil);

            if (estadoExiste is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryEstadoCivil.Actualizar(estadoCivil);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Borrar(int IdEstadoCivil)
        {
            var estadoCivil = await repositoryEstadoCivil.ObtenerPorId(IdEstadoCivil);

            if (estadoCivil is null)
            {
                return RedirectToAction("Index", "EstadoCivil");
            }
            return View(estadoCivil);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarEstado(int IdEstadoCivil)
        {
            var estado = await repositoryEstadoCivil.ObtenerPorId(IdEstadoCivil);
            if (estado is null)
            {
                return RedirectToAction("Index", "Home");
            }
            await repositoryEstadoCivil.Borrar(IdEstadoCivil);
            return RedirectToAction("Index");
        }
    }
}
