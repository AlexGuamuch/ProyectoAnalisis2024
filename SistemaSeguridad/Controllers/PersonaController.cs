﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public PersonaController(
            IRepositoryPersona repositoryPersona,
            IServicioUsuarios servicioUsuarios,
            IRepositoryGenero repositoryGenero,
            IRepositoryEstadoCivil repositoryEstadoCivil)
        {
            this.repositoryPersona = repositoryPersona;
            this.servicioUsuarios = servicioUsuarios;
            this.repositoryGenero = repositoryGenero;
            this.repositoryEstadoCivil = repositoryEstadoCivil;
        }

        public async Task<IActionResult> Index()
        {
            var personas = await repositoryPersona.Obtener();
            return View(personas);
        }

        public async Task<IActionResult> Crear()
        {
            await CargarGenerosYEstadosCiviles();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Persona persona)
        {
            if (!ModelState.IsValid)
            {
                await CargarGenerosYEstadosCiviles();
                return View(persona);
            }

            try
            {
                persona.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();
                persona.FechaCreacion = DateTime.Now;
                await repositoryPersona.Crear(persona);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear la persona: " + ex.Message);
                await CargarGenerosYEstadosCiviles();
                return View(persona);
            }
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
    }
}
