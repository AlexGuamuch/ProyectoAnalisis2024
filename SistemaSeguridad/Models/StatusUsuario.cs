namespace SistemaSeguridad.Models
{
	public class StatusUsuario
	{
		public int IdStatusUsuario { get; set; }
		public string Nombre { get; set; }
		public DateTime FechaCreacion { get; set; }
		public string UsuarioCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public string UsuarioModificacion { get; set; }

	}
}
