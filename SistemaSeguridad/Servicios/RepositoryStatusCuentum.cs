using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryStatusCuentum
    {
        Task<IEnumerable<StatusCuentum>> ObtenerTodos();
        Task<StatusCuentum> ObtenerPorId(int idStatusCuenta);
    }

    public class RepositoryStatusCuentum : IRepositoryStatusCuentum
    {
        private readonly string connectionString;

        public RepositoryStatusCuentum(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<StatusCuentum>> ObtenerTodos()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<StatusCuentum>(@"
                SELECT IdStatusCuenta, Nombre, FechaCreacion, 
                       UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM STATUS_CUENTA;"); 
        }

        public async Task<StatusCuentum> ObtenerPorId(int idStatusCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<StatusCuentum>(@"
                SELECT IdStatusCuenta, Nombre, FechaCreacion, 
                       UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM STATUS_CUENTA 
                WHERE IdStatusCuenta = @IdStatusCuenta;",
                new { IdStatusCuenta = idStatusCuenta });
        }
    }
}
