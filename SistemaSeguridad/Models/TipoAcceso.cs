namespace SistemaSeguridad.Models
{
	public class TipoAcceso
	{
		public int IdTipoAcceso { get; set; }
		public string Nombre { get; set; }
		public DateTime FechaCreacion { get; set; }
		public string UsuarioCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public string UsuarioModificacion { get; set; }

	}
}
