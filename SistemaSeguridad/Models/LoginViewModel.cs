using System.ComponentModel.DataAnnotations;

namespace SistemaSeguridad.Models
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "El campo {0} es requerido")]
		public string IdUsuario { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		public bool Recuerdame { get; set; }
	}
}
