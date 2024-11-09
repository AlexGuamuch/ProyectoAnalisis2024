namespace SistemaSeguridad.Models
{
	public class UsuarioRole
	{
		public string IdUsuario { get; set; }
		public int IdRole { get; set; }
		public DateTime FechaCreacion { get; set; }
        public string UsuarioCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public string UsuarioModificacion { get; set; }
    }
}
