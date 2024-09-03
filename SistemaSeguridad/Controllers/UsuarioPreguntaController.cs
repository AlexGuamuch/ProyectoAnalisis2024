using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaSeguridad.Controllers
{
    public class UsuarioPreguntaController : Controller
    {
        private readonly UserManager<UsuarioPrueba> _userManager;
        // private readonly IRepositoryUsuarioPregunta repositoryUsuarioPregunta;
        private readonly IRepositoryUsuarioPregunta _usuarioPreguntaRepository;

        public UsuarioPreguntaController(UserManager<UsuarioPrueba> userManager, IRepositoryUsuarioPregunta usuarioPreguntaRepository)
        {
            _userManager = userManager;
            _usuarioPreguntaRepository = usuarioPreguntaRepository;

        }

        public async Task<IActionResult> Crear()
        {
            var usuarioLogin = await _userManager.GetUserAsync(User);
            if (usuarioLogin == null)
            {
                return RedirectToAction("Login", "UsuarioLogin");
            }

            return View("UsuarioPregunta", new UsuarioPregunta { IdUsuario = usuarioLogin.IdUsuario });
        }

        [HttpPost]
        public async Task<IActionResult> Crear(string respuesta1, string respuesta2, string respuesta3, string respuesta4)
        {
            var usuarioLogin = await _userManager.GetUserAsync(User);
            if (usuarioLogin == null)
            {
                return RedirectToAction("Login", "UsuarioLogin");
            }

            var preguntasUsuario = await _usuarioPreguntaRepository.ObtenerPorUsuario(usuarioLogin.IdUsuario);
            if (preguntasUsuario.Count >= 4)
            {
                ViewBag.Message = "Ya has ingresado las 4 preguntas de seguridad.";
                return View("UsuarioPregunta", new UsuarioPregunta { IdUsuario = usuarioLogin.IdUsuario });
            }

            if (ModelState.IsValid)
            {
                var preguntas = new List<UsuarioPregunta>
                {
                    new UsuarioPregunta
                    {
                        IdUsuario = usuarioLogin.IdUsuario,
                        Pregunta = "¿Nombre de tu primera mascota?",
                        Respuesta = respuesta1,
                        OrdenPregunta = 1,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreacion = usuarioLogin.IdUsuario
                    },
                    new UsuarioPregunta
                    {
                        IdUsuario = usuarioLogin.IdUsuario,
                        Pregunta = "¿Lugar de nacimiento de tu madre?",
                        Respuesta = respuesta2,
                        OrdenPregunta = 2,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreacion = usuarioLogin.IdUsuario
                    },
                    new UsuarioPregunta
                    {
                        IdUsuario = usuarioLogin.IdUsuario,
                        Pregunta = "¿Nombre del catedrático del curso de Análisis de Sistemas II?",
                        Respuesta = respuesta3,
                        OrdenPregunta = 3,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreacion = usuarioLogin.IdUsuario
                    },
                    new UsuarioPregunta
                    {
                        IdUsuario = usuarioLogin.IdUsuario,
                        Pregunta = "¿Nombre de tu curso preferido?",
                        Respuesta = respuesta4,
                        OrdenPregunta = 4,
                        FechaCreacion = DateTime.Now,
                        UsuarioCreacion = usuarioLogin.IdUsuario
                    }
                };
                // Verifica que el repositorio no sea null
                if (_usuarioPreguntaRepository == null)
                {
                    throw new InvalidOperationException("El repositorio de preguntas de usuario no ha sido inicializado.");
                }


                foreach (var pregunta in preguntas)
                {
                    await _usuarioPreguntaRepository.Crear(pregunta);
                }

                TempData["Message"] = "Preguntas guardadas con éxito";
                return RedirectToAction("Perfil", "Usuario");
            }

            return View("UsuarioPregunta", new UsuarioPregunta { IdUsuario = usuarioLogin.IdUsuario });
        }
    }
}