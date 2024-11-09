using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaSeguridad.Models
{
    public class OpcionCreacionViewModel: Opcion
    {
        public IEnumerable<SelectListItem> Menu { get; set; }
    }
}
