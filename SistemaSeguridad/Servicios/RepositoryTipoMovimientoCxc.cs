using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryTipoMovimientoCxc
    {
        Task<IEnumerable<TipoMovimientoCxc>> ObtenerTodos();
        Task<TipoMovimientoCxc> ObtenerPorId(int idTipoMovimientoCxc);
    }

    public class RepositoryTipoMovimientoCxc : IRepositoryTipoMovimientoCxc
    {
        private readonly string connectionString;

        public RepositoryTipoMovimientoCxc(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<TipoMovimientoCxc>> ObtenerTodos()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoMovimientoCxc>(@"
                SELECT IdTipoMovimientoCxc, Nombre, OperacionCuentaCorriente, 
                       FechaCreacion, UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM TIPO_MOVIMIENTO_CXC;");
        }

        public async Task<TipoMovimientoCxc> ObtenerPorId(int idTipoMovimientoCxc)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoMovimientoCxc>(@"
                SELECT IdTipoMovimientoCxc, Nombre, OperacionCuentaCorriente, 
                       FechaCreacion, UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM TIPO_MOVIMIENTO_CXC 
                WHERE IdTipoMovimientoCxc = @IdTipoMovimientoCxc;",
                new { IdTipoMovimientoCxc = idTipoMovimientoCxc });
        }
    }
}
