using Microsoft.AspNetCore.Mvc;
using SistemaSeguridad.Servicios;

namespace SistemaSeguridad.Controllers
{
    public class CierreController : Controller
    {
        private readonly CierreService _cierreService; 
        public CierreController(CierreService cierreService) 
        { _cierreService = cierreService; }
        
        [HttpPost]
        public async Task<IActionResult> EjecutarCierre(DateTime fechaInicio, DateTime fechaCierre)
        { 
            await _cierreService.EjecutarProcedimientoCierre(fechaInicio, fechaCierre); 
            return Json(new { success = true, message = "El cierre se realizó con éxito." }); 
        }
    }
}