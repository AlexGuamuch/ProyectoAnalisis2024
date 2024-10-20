using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryTipoDocumento
    {
        Task<IEnumerable<TipoDocumento>> ObtenerTodos();
        Task<TipoDocumento> ObtenerPorId(int idTipoDocumento);
    }

    public class RepositoryTipoDocumento : IRepositoryTipoDocumento
    {
        private readonly string connectionString;

        public RepositoryTipoDocumento(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<TipoDocumento>> ObtenerTodos()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoDocumento>(@"
                SELECT IdTipoDocumento, Nombre, FechaCreacion, 
                       UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM TIPO_DOCUMENTO;");
        }

        public async Task<TipoDocumento> ObtenerPorId(int idTipoDocumento)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoDocumento>(@"
                SELECT IdTipoDocumento, Nombre, FechaCreacion, 
                       UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM TIPO_DOCUMENTO 
                WHERE IdTipoDocumento = @IdTipoDocumento;",
                new { IdTipoDocumento = idTipoDocumento });
        }
    }
}
