using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryTipoSaldoCuentum
    {
        Task<IEnumerable<TipoSaldoCuentum>> ObtenerTodos();
        Task<TipoSaldoCuentum> ObtenerPorId(int idTipoSaldoCuenta);
    }

    public class RepositoryTipoSaldoCuentum : IRepositoryTipoSaldoCuentum
    {
        private readonly string connectionString;

        public RepositoryTipoSaldoCuentum(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<TipoSaldoCuentum>> ObtenerTodos()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoSaldoCuentum>(@"
                SELECT IdTipoSaldoCuenta, Nombre, 
                       FechaCreacion, UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM TIPO_SALDO_CUENTA;");
        }

        public async Task<TipoSaldoCuentum> ObtenerPorId(int idTipoSaldoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoSaldoCuentum>(@"
                SELECT IdTipoSaldoCuenta, Nombre, 
                       FechaCreacion, UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM TIPO_SALDO_CUENTA 
                WHERE IdTipoSaldoCuenta = @IdTipoSaldoCuenta;",
                new { IdTipoSaldoCuenta = idTipoSaldoCuenta });
        }
    }
}
