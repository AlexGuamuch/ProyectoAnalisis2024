namespace SistemaSeguridad.Models
{
	public class Modulo
	{
		public int IdModulo { get; set; }
		public string Nombre { get; set; }
		public int OrdenMenu { get; set; }
		public DateTime FechaCreacion { get; set; }
		public string UsuarioCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public string UsuarioModificacion { get; set; }

	}
}
