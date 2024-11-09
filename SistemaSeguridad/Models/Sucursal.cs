using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SistemaSeguridad.Models
{
	public class Sucursal
	{
		public int IdSucursal { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Remote(action: "VerifarSucursal", controller: "Sucursal")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string Direccion { get; set; }
		public int IdEmpresa { get; set; }
		public DateTime FechaCreacion { get; set; }
		public string UsuarioCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public string UsuarioModificacion { get; set; }

	}
}
