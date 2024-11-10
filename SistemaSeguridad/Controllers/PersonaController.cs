using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;
using System.Threading.Tasks;

namespace SistemaSeguridad.Controllers
{
    public class PersonaController : Controller
    {
        private readonly IRepositoryPersona repositoryPersona;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositoryGenero repositoryGenero;
        private readonly IRepositoryEstadoCivil repositoryEstadoCivil;
        private readonly IRepositorySaldoCuenta repositorySaldoCuenta;
        private readonly IRepositoryMovimientoCuenta repositoryMovimientoCuenta;

        public PersonaController(
            IRepositoryPersona repositoryPersona,
            IServicioUsuarios servicioUsuarios,
            IRepositoryGenero repositoryGenero,
            IRepositoryEstadoCivil repositoryEstadoCivil,
            IRepositorySaldoCuenta repositorySaldoCuenta,
            IRepositoryMovimientoCuenta repositoryMovimientoCuenta)
        {
            this.repositoryPersona = repositoryPersona;
            this.servicioUsuarios = servicioUsuarios;
            this.repositoryGenero = repositoryGenero;
            this.repositoryEstadoCivil = repositoryEstadoCivil;
            this.repositorySaldoCuenta = repositorySaldoCuenta;
            this.repositoryMovimientoCuenta = repositoryMovimientoCuenta;
        }

        public async Task<IActionResult> Index()
        {
            var personas = await repositoryPersona.Obtener();
            return View(personas);
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerGenero()
        {
            var modulo = await repositoryGenero.Obtener();
            return modulo.Select(x => new SelectListItem(x.Nombre, x.IdGenero.ToString()));
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerEstadoCivil()
        {
            var modulo = await repositoryEstadoCivil.Obtener();
            return modulo.Select(x => new SelectListItem(x.Nombre, x.IdEstadoCivil.ToString()));
        }


        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var modelo = new PersonaCreacionViewModel();
            modelo.Genero = await ObtenerGenero();
            modelo.EstadoCivil = await ObtenerEstadoCivil();
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(PersonaCreacionViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            modelo.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();
            modelo.Genero = await ObtenerGenero();
            modelo.EstadoCivil = await ObtenerEstadoCivil();

            await repositoryPersona.Crear(modelo);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int idPersona)
        {
            var persona = await repositoryPersona.ObtenerPorId(idPersona);
            if (persona is null)
            {
                return RedirectToAction("Index");
            }

            await CargarGenerosYEstadosCiviles();
            return View(persona);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Persona persona)
        {
            if (!ModelState.IsValid)
            {
                await CargarGenerosYEstadosCiviles();
                return View(persona);
            }

            try
            {
                persona.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
                persona.FechaModificacion = DateTime.Now;

                var personaExistente = await repositoryPersona.ObtenerPorId(persona.IdPersona);
                if (personaExistente is null)
                {
                    return RedirectToAction("Index");
                }

                await repositoryPersona.Actualizar(persona);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al editar la persona: " + ex.Message);
                await CargarGenerosYEstadosCiviles();
                return View(persona);
            }
        }

        public async Task<IActionResult> Borrar(int idPersona)
        {
            var persona = await repositoryPersona.ObtenerPorId(idPersona);
            if (persona is null)
            {
                return RedirectToAction("Index");
            }

            return View(persona);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarPersona(int idPersona) // Cambiado el nombre del método
        {
            var persona = await repositoryPersona.ObtenerPorId(idPersona);
            if (persona is null)
            {
                return RedirectToAction("Index");
            }

            await repositoryPersona.Borrar(idPersona);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExportarCSV()
        {
            var personas = await repositoryPersona.Obtener();
            var csv = new StringBuilder();
            csv.AppendLine("IdPersona,Nombre,Apellido,FechaNacimiento,IdGenero,Direccion,Telefono,CorreoElectronico,IdEstadoCivil,FechaCreacion,UsuarioCreacion");

            foreach (var persona in personas)
            {
                csv.AppendLine($"{persona.IdPersona},{persona.Nombre},{persona.Apellido}," +
                               $"{persona.FechaNacimiento.ToShortDateString()},{persona.IdGenero}," +
                               $"{persona.Direccion},{persona.Telefono},{persona.CorreoElectronico}," +
                               $"{persona.IdEstadoCivil},{persona.FechaCreacion},{persona.UsuarioCreacion}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "personas.csv");
        }

        private async Task CargarGenerosYEstadosCiviles()
        {
            var generos = await repositoryGenero.Obtener();
            ViewBag.Generos = generos.Select(g => new SelectListItem
            {
                Value = g.IdGenero.ToString(),
                Text = g.Nombre // Cambia 'Nombre' por la propiedad que desees mostrar
            }).ToList();

            var estadosCiviles = await repositoryEstadoCivil.ObtenerTodos();
            ViewBag.EstadosCiviles = estadosCiviles.Select(ec => new SelectListItem
            {
                Value = ec.IdEstadoCivil.ToString(),
                Text = ec.Nombre // Cambia 'Nombre' por la propiedad que desees mostrar
            }).ToList();
        }

        [HttpGet]
        public IActionResult Busqueda()
        {
            return View(Enumerable.Empty<Persona>());
        }

        [HttpPost]
        public async Task<IActionResult> Busqueda(string tipoBusqueda, string valorBusqueda)
        {
            if (string.IsNullOrWhiteSpace(valorBusqueda))
            {
                ModelState.AddModelError(string.Empty, "Por favor, ingrese un valor de búsqueda.");
                return View(Enumerable.Empty<Persona>());
            }
            IEnumerable<Persona> resultados;

            switch (tipoBusqueda)
            {
                case "Nombre":
                    resultados = await repositoryPersona.BuscarPorNombre(valorBusqueda);
                    break;
                case "Identificacion":
                    resultados = await repositoryPersona.BuscarPorDpi(valorBusqueda);
                    break;
                default:
                    ModelState.AddModelError(string.Empty, "Tipo de búsqueda no válido.");
                    return View(Enumerable.Empty<Persona>());
            }
            return View(resultados);
        }

        [HttpGet]
        public async Task<IActionResult> Cuentas(int idPersona)
        {
            var persona = await repositoryPersona.ObtenerPorId(idPersona);
            if (persona is null)
            {
                return RedirectToAction("Index");
            }

            var saldoCuentas = await repositorySaldoCuenta.ObtenerCuentasId(idPersona);

            var viewModel = new SaldoCuentumViewModel
            {
                Persona = persona,
                SaldoCuenta = saldoCuentas
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Movimientos(int idSaldoCuenta)
        {
            var saldoCuenta = await repositorySaldoCuenta.ObtenerPorId(idSaldoCuenta);
            if (saldoCuenta is null)
            {
                return RedirectToAction("Index");
            }

            var movimientoCuenta = await repositoryMovimientoCuenta.ObtenerMovimientosId(idSaldoCuenta);

            var viewModel = new MovimientoViewModel
            {
                SaldoCuentum = saldoCuenta,
                MovimientoCuenta = movimientoCuenta
            };

            return View(viewModel);
        }


    }
}
