using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaSeguridad.Models
{
    public class SucursalCreacionViewModel: Sucursal
    {
        public IEnumerable<SelectListItem> Empresa { get; set; }
    }
}
