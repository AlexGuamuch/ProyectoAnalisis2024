namespace SistemaSeguridad.Models
{
	public class RoleOpcion
	{
		public int IdRole { get; set; }
		public int IdOpcion { get; set; }
		public int Alta { get; set; }
		public int Baja { get; set; }
		public int Cambio { get; set; }
		public int Imprimir { get; set; }
		public int Exportar { get; set; }
		public DateTime FechaCreacion { get; set; }
		public string UsuarioCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public string UsuarioModificacion { get; set; }

	}
}
