namespace SistemaSeguridad.Models
{
	public class Opcion
	{
		public int IdOpcion { get; set; }
		public int IdMenu { get; set; }
		public string Nombre { get; set; }
		public int OrdenMenu { get; set; }
		public string Pagina { get; set; }
		public DateTime FechaCreacion { get; set; }
		public string UsuarioCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public string UsuarioModificacion { get; set; }

	}
}
