using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaSeguridad.Models
{
    public class RegistroCreacionViewModel: RegistroViewModel
    {
        public IEnumerable<SelectListItem> Genero { get; set; }
		public IEnumerable<SelectListItem> Sucursal { get; set; }

	}
}
