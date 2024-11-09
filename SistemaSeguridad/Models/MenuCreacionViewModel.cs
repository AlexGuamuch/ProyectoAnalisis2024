using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaSeguridad.Models
{
    public class MenuCreacionViewModel: Menu
    {
        public IEnumerable<SelectListItem> Modulo { get; set; }
    }
}
