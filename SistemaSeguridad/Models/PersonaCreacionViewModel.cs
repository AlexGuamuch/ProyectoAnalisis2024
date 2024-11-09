using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaSeguridad.Models
{
    public class PersonaCreacionViewModel: Persona
    {
        public IEnumerable<SelectListItem> Genero { get; set; }
        public IEnumerable<SelectListItem> EstadoCivil { get; set; }
    }
}
