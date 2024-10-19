using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryEstadoCivil
    {
        Task<IEnumerable<EstadoCivil>> ObtenerTodos();
        Task<EstadoCivil> ObtenerPorId(int idEstadoCivil);
    }

    public class RepositoryEstadoCivil : IRepositoryEstadoCivil
    {
        private readonly string connectionString;

        public RepositoryEstadoCivil(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<EstadoCivil>> ObtenerTodos()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<EstadoCivil>(@"
                SELECT IdEstadoCivil, Nombre, FechaCreacion, 
                       UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM ESTADO_CIVIL;"); // Asegúrate de que el nombre de la tabla es correcto
        }

        public async Task<EstadoCivil> ObtenerPorId(int idEstadoCivil)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<EstadoCivil>(@"
                SELECT IdEstadoCivil, Nombre, FechaCreacion, 
                       UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM ESTADO_CIVIL 
                WHERE IdEstadoCivil = @IdEstadoCivil;",
                new { IdEstadoCivil = idEstadoCivil });
        }
    }
}
