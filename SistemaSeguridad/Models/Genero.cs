using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SistemaSeguridad.Models
{
	public class Genero
	{
		//Modelo de base de datos
		public int IdGenero { get; set; }
		//Required se usa para que un campo sea requerido
		[Required(ErrorMessage = "El campo {0} es requerido")]
		//Display se usa solo para darle otro nombre al campo en la vista 
		[Display(Name = "Nombre Genero")]
		//Remote permite acceder a metodos creados en el Controller, validaciones perzonalizadas
		[Remote(action: "VerifarGenero", controller:"Genero")]
		public string Nombre { get; set; }
		public DateTime FechaCreacion { get; set; }
		public string UsuarioCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public string UsuarioModificacion { get; set; }

	}
}
