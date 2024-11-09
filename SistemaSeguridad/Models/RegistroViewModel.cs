using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SistemaSeguridad.Models
{
	public class RegistroViewModel
	{
		[Required(ErrorMessage = "El campo {0} es requerido")]
        [Remote(action: "VerifarUsuario", controller: "Usuario")]
        public string IdUsuario { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		public string Nombre { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		public string Apellido { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[Display(Name = "Fecha Nacimiento")]
		[DataType(DataType.Date)]
		public DateTime FechaNacimiento { get; set; } = DateTime.Today;
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[EmailAddress(ErrorMessage = "El campo debe ser un correo valido")]
		public string CorreoElectronico { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public string TelefonoMovil { get; set; }
		public string UsuarioCreacion { get; set; }
        public int IdGenero { get; set; }
		public int IdSucursal { get; set; }
		
		[Display(Name = "Fotografía")]
		public IFormFile Fotografia { get; set; }
	}
}
