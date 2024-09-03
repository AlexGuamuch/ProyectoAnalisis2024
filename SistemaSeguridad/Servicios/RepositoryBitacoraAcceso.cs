using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
	public interface IRepositoryBitacoraAcceso
	{
		Task Bitacora(BitacoraAcceso bitacora);
	}
	public class RepositoryBitacoraAcceso: IRepositoryBitacoraAcceso
	{
		public readonly string connectionString;
		public RepositoryBitacoraAcceso(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public async Task Bitacora(BitacoraAcceso bitacora)
		{
			using var connection = new SqlConnection(connectionString);
			var id = await connection.QuerySingleAsync<int>(@"insert into
															BITACORA_ACCESO(IdUsuario,IdTipoAcceso,FechaAcceso,HttpUserAgent,DireccionIp,Accion,SistemaOperativo,Dispositivo,Browser)
															values(@IdUsuario,1,GETDATE(),@HttpUserAgent,@DireccionIp,@Accion,@SistemaOperativo,@Dispositivo,@Browser);
															select SCOPE_IDENTITY();", bitacora);
			bitacora.IdTipoAcceso = id;
		}
	}
}
