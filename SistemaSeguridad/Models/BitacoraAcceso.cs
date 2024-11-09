namespace SistemaSeguridad.Models
{
	public class BitacoraAcceso
	{
		public int IdBitacoraAcceso { get; set; }
		public string IdUsuario { get; set; }
		public int IdTipoAcceso { get; set; }
		public DateTime FechaAcceso { get; set; }
		public string HttpUserAgent { get; set; }
		public string DireccionIp { get; set; }
		public string Accion { get; set; }
		public string SistemaOperativo { get; set; }
		public string Dispositivo { get; set; }
		public string Browser { get; set; }
		public string Sesion { get; set; }

	}
}
