namespace SistemaSeguridad.Models
{
	public class UsuarioPrueba
	{
        public string IdUsuario { get; set; }
		public string Nombre { get; set; }
		public string Apellido { get; set; }
		public DateTime FechaNacimiento { get; set; }
		public string CorreoElectronico { get; set; }
		public string Password { get; set; }
		public string Fotografia { get; set; }		
		public int IdGenero { get; set; }
		public int IdSucursal { get; set; }
		public string TelefonoMovil { get; set; }
		public string UsuarioCreacion { get; set; }
		public int IntentosDeAcceso { get; set; }
		public DateTime? FechaDesbloqueo { get; set; }
		public int IdStatusUsuario { get; set; }
	}
}
