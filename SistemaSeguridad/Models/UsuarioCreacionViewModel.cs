using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaSeguridad.Models
{
    public class UsuarioCreacionViewModel: UsuarioRole
    {
        public IEnumerable<SelectListItem> Usuarios { get; set; }
		public IEnumerable<SelectListItem> Roles { get; set; }


	}
}
