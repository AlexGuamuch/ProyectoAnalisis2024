using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SistemaSeguridad.Models
{
	public class Empresa
	{
        public int IdEmpresa { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		[Remote(action: "VerifarEmpresa", controller: "Empresa")]
		public string Nombre { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		public string Direccion { get; set; }
		[Required(ErrorMessage = "El campo {0} es requerido")]
		public string Nit { get; set; }
		[Display(Name = "Cantidad de Mayusculas")]
        public int PasswordCantidadMayusculas { get; set; }
        [Display(Name = "Cantidad de Minusculas")]
        public int PasswordCantidadMinusculas { get; set; }
        [Display(Name = "Cantidad de Caracteres Especiales")]
        public int PasswordCantidadCaracteresEspeciales { get; set; }
        [Display(Name = "Cantidad de Caducidad de Dias")]
        public int PasswordCantidadCaducidadDias { get; set; }
        [Display(Name = "Largo de Password")]
        public int PasswordLargo { get; set; }
        [Display(Name = "Cantidad de Intentos antes de bloquear")]
        public int PasswordIntentosAntesDeBloquear { get; set; }
        [Display(Name = "Cantididad de Numeros")]
        public int PasswordCantidadNumeros { get; set; }
        [Display(Name = "Cantidad de Preguntas a validar")]
        public int PasswordCantidadPreguntasValidar { get; set; }
		public DateTime FechaCreacion { get; set; }
		public string UsuarioCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public string UsuarioModificacion { get; set; }

	}
}
