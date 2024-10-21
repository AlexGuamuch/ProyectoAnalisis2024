using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaSeguridad.Models;
using SistemaSeguridad.Servicios;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaSeguridad.Controllers
{
    public class CuentaController : Controller
    {
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositoryCuentaCorriente repositoryCuentaCorriente;
        private readonly IRepositoryPersona repositoryPersona;

        public CuentaController(IServicioUsuarios servicioUsuarios,
                                IRepositoryCuentaCorriente repositoryCuentaCorriente,
                                IRepositoryPersona repositoryPersona)
        {
            this.servicioUsuarios = servicioUsuarios;
            this.repositoryCuentaCorriente = repositoryCuentaCorriente;
            this.repositoryPersona = repositoryPersona;
        }

        [HttpGet]
        public async Task<IActionResult> RegistrarMovimiento()
        {
            var clientes = await repositoryPersona.ObtenerClientes();
            ViewBag.Clientes = new SelectList(clientes, "IdPersona", "NombreCompleto");
            return View(new MovimientoCuentum());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarMovimiento(MovimientoCuentum movimiento)
        {
            if (!ModelState.IsValid)
            {
                var clientes = await repositoryPersona.ObtenerClientes();
                ViewBag.Clientes = new SelectList(clientes, "IdPersona", "NombreCompleto");
                return View(movimiento);
            }

            try
            {
                movimiento.FechaMovimiento = DateTime.Now;
                movimiento.FechaCreacion = DateTime.Now;
                movimiento.UsuarioCreacion = servicioUsuarios.ObtenerUsuarioId();
                movimiento.GeneradoAutomaticamente = false;
                movimiento.ValorMovimientoPagado = 0;

                await repositoryCuentaCorriente.RegistrarMovimiento(movimiento);
                TempData["SuccessMessage"] = "Movimiento registrado correctamente.";
                return RedirectToAction("CuentasPorCliente", new { idPersona = movimiento.IdSaldoCuenta });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Ocurrió un error al registrar el movimiento: " + ex.Message);
                var clientes = await repositoryPersona.ObtenerClientes();
                ViewBag.Clientes = new SelectList(clientes, "IdPersona", "NombreCompleto");
                return View(movimiento);
            }
        }
        public async Task<IActionResult> CuentasPorCliente(int idPersona)
        {
            var cuentas = await repositoryCuentaCorriente.BuscarCuentaPorPersona(idPersona.ToString());
            return View(cuentas);
        }

        public async Task<IActionResult> TodasLasCuentas()
        {
            var cuentas = await repositoryCuentaCorriente.ObtenerCuentas();
            return View(cuentas);
        }
    }
}
