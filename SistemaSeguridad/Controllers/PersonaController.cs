using Microsoft.AspNetCore.Mvc;
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
        private readonly IRepositoryGenero repositoryGenero; // Añadido
        private readonly IRepositoryEstadoCivil repositoryEstadoCivil; // Añadido

        public PersonaController(
            IRepositoryPersona repositoryPersona,
            IServicioUsuarios servicioUsuarios,
            IRepositoryGenero repositoryGenero, // Añadido
            IRepositoryEstadoCivil repositoryEstadoCivil) // Añadido
        {
            this.repositoryPersona = repositoryPersona;
            this.servicioUsuarios = servicioUsuarios;
            this.repositoryGenero = repositoryGenero; // Añadido
            this.repositoryEstadoCivil = repositoryEstadoCivil; // Añadido
        }

        public async Task<IActionResult> Index()
        {
            var personas = await repositoryPersona.Obtener();
            return View(personas);
        }

        public async Task<IActionResult> Crear()
        {
            await CargarGenerosYEstadosCiviles(); // Cargar datos en la vista Crear
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Persona persona)
        {
            if (!ModelState.IsValid)
            {
                await CargarGenerosYEstadosCiviles(); // Cargar datos si hay errores
                return View(persona);
            }

            persona.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();
            persona.FechaCreacion = DateTime.Now; // Asigna la fecha de creación

            await repositoryPersona.Crear(persona);
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

            await CargarGenerosYEstadosCiviles(); // Cargar datos para el dropdown en la vista Editar
            return View(persona);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Persona persona)
        {
            if (!ModelState.IsValid)
            {
                await CargarGenerosYEstadosCiviles(); // Cargar datos si hay errores
                return View(persona);
            }

            persona.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            persona.FechaModificacion = DateTime.Now; // Asigna la fecha de modificación

            var personaExistente = await repositoryPersona.ObtenerPorId(persona.IdPersona);

            if (personaExistente is null)
            {
                return RedirectToAction("Index");
            }

            await repositoryPersona.Actualizar(persona);
            return RedirectToAction("Index");
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
        public async Task<IActionResult> BorrarPersona(int idPersona)
        {
            var persona = await repositoryPersona.ObtenerPorId(idPersona);

            if (persona is null)
            {
                return RedirectToAction("Index");
            }

            await repositoryPersona.Borrar(idPersona);
            return RedirectToAction("Index");
        }

        // Método para exportar a CSV
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

        // Método para cargar géneros y estados civiles
        private async Task CargarGenerosYEstadosCiviles()
        {
            ViewBag.Generos = await repositoryGenero.ObtenerTodos(); // Método para obtener todos los géneros
            ViewBag.EstadosCiviles = await repositoryEstadoCivil.ObtenerTodos(); // Método para obtener todos los estados civiles
        }
    }
}
