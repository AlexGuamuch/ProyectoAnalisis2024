using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System.Text;

namespace SistemaSeguridad.Controllers
{
    public class MenuController : Controller
    {
        private readonly IRepositoryMenu repositoryMenu;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositoryModulo repositoryModulo;

        public MenuController(IRepositoryMenu repositoryMenu, IServicioUsuarios servicioUsuarios,
                                IRepositoryModulo repositoryModulo)
        {
            this.repositoryMenu = repositoryMenu;
            this.servicioUsuarios = servicioUsuarios;
            this.repositoryModulo = repositoryModulo;
        }

        public async Task<IActionResult> Index()
        {
            var menu = await repositoryMenu.Obtener();
            return View(menu);
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerModulo()
        {
            var modulo = await repositoryModulo.Obtener();
            return modulo.Select(x => new SelectListItem(x.Nombre, x.IdModulo.ToString()));
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var modelo = new MenuCreacionViewModel();
            modelo.Modulo = await ObtenerModulo();
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(MenuCreacionViewModel menu)
        {
            if (!ModelState.IsValid)
            {
                return View(menu);
            }

            menu.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();
            menu.Modulo = await ObtenerModulo();

            var existeMenu = await repositoryMenu.Existe(menu.Nombre);

            if (existeMenu)
            {
                ModelState.AddModelError(nameof(menu.Nombre), $"El nombre {menu.Nombre} ya existe");
                return View(menu);
            }

            await repositoryMenu.Crear(menu);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> VerifarMenu(string nombre)
        {
            var existeMenu = await repositoryMenu.Existe(nombre);
            if (existeMenu)
            {
                return Json($"El nombre {nombre} ya existe");
            }

            return Json(true);
        }

        public async Task<IActionResult> Borrar(int idMenu)
        {
            var menu = await repositoryMenu.ObtenerPorId(idMenu);

            if (menu is null)
            {
                return RedirectToAction("Index", "Menu");
            }
            return View(menu);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarMenu(int idMenu)
        {
            try
            {
                var menu = await repositoryMenu.ObtenerPorId(idMenu);
                if (menu is null)
                {
                    return RedirectToAction("Index", "Menu");
                }
                await repositoryMenu.Borrar(idMenu);
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex + "No se puede borrar este registro");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Editar(int idMenu)
        {
            var menu = await repositoryMenu.ObtenerPorId(idMenu);

            if (menu is null)
            {
                return RedirectToAction("Index", "Menu");
            }

            return View(menu);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(Menu menu)
        {
            menu.UsuarioModificacion = servicioUsuarios.ObtenerUsuarioId();
            var menuExiste = await repositoryMenu.ObtenerPorId(menu.IdMenu);

            if (menu is null)
            {
                return RedirectToAction("Index", "Menu");
            }

            await repositoryMenu.ActualizarGeneral(menu);
            return RedirectToAction("Index");
        }

        // función para exportar a CSV
        [HttpGet]
        public async Task<IActionResult> ExportarCSV()
        {
            var menus = await repositoryMenu.Obtener();

            var builder = new StringBuilder();
            builder.AppendLine("IdMenu,Nombre");

            foreach (var menu in menus)
            {
                builder.AppendLine($"{menu.IdMenu},{menu.Nombre}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "Menus.csv");
        }
    }
}
