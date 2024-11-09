using System.ComponentModel.DataAnnotations;

namespace SistemaSeguridad.Models
{
	public class Usuario
	{
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[Display(Name = "Nombre de Usuario")]
		public string IdUsuario { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Fecha de Nacimiento")]
		[DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; } = DateTime.Now;
        public int IdStatusUsuario { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
		[DataType(DataType.Password)]
        public string Password { get; set; }
		[Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar un genero")]
		[Display(Name = "Genero")]
        public int IdGenero { get; set; }
		public DateTime UltimaFechaIngreso { get; set; }
		public int IntentosDeAcceso { get; set; }
		public string SesionActual { get; set; }
		public DateTime UltimaFechaCambioPassword { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
		[EmailAddress(ErrorMessage = "El campo debe ser un correo electronico valido")]
        public string CorreoElectronico { get; set; }
		public int RequiereCambiarPassword { get; set; }
		public string Fotografia { get; set; }
		public string TelefonoMovil { get; set; }
		[Display(Name = "Sucursal")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una sucursal")]
        public int IdSucursal { get; set; }
		public DateTime FechaCreacion { get; set; }
		public string UsuarioCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public string UsuarioModificacion { get; set; }

	}
}
