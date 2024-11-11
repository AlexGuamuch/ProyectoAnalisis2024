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
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                // Insertar en BITACORA_ACCESO
                var id = await connection.QuerySingleAsync<int>(@"
            INSERT INTO BITACORA_ACCESO (IdUsuario, IdTipoAcceso, FechaAcceso, HttpUserAgent, DireccionIp, Accion, SistemaOperativo, Dispositivo, Browser)
            VALUES (@IdUsuario, 1, GETDATE(), @HttpUserAgent, @DireccionIp, @Accion, @SistemaOperativo, @Dispositivo, @Browser);
            SELECT SCOPE_IDENTITY();", bitacora, transaction);

                bitacora.IdTipoAcceso = id;

                // Ahora insertar en la tabla USUARIO
                await connection.ExecuteAsync(@"
            UPDATE USUARIO 
            SET UltimaFechaIngreso = GETDATE(), 
            SesionActual = @SesionActual 
            WHERE IdUsuario = @IdUsuario;",
                    new { SesionActual = "Activa", IdUsuario = bitacora.IdUsuario }, transaction);

                // Confirmar la transacción
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Si hay un error, revertir la transacción
                transaction.Rollback();
                throw; // O maneja el error según sea necesario
            }
            //bitacora.IdTipoAcceso = id;
		}
	}
}
