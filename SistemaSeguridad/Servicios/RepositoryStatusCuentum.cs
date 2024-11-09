using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryStatusCuentum
    {
        Task<IEnumerable<StatusCuentum>> ObtenerTodos();
        Task<StatusCuentum> ObtenerPorId(int idStatusCuenta);
        Task Crear(StatusCuentum statusCuentum);
        Task Actualizar(StatusCuentum statusCuentum);
        Task Borrar(int idStatusCuenta);
        Task<bool> Existe(string nombre);
    }

    public class RepositoryStatusCuentum : IRepositoryStatusCuentum
    {
        private readonly string connectionString;

        public RepositoryStatusCuentum(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(StatusCuentum statusCuentum)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("spestatuscuentainserta",
                                                             new
                                                             {
                                                                 Nombre = statusCuentum.Nombre,
                                                                 UsuarioCreacion = statusCuentum.UsuarioCreacion
                                                             },
                                                             commandType: System.Data.CommandType.StoredProcedure);
            statusCuentum.IdStatusCuenta = id;
        }


        public async Task<IEnumerable<StatusCuentum>> ObtenerTodos()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<StatusCuentum>(@"SELECT IdStatusCuenta, Nombre, FechaCreacion FROM STATUS_CUENTA;"); 
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

        public async Task Actualizar(StatusCuentum statusCuentum)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update STATUS_CUENTA
                                            set Nombre = @Nombre,FechaModificacion = GETDATE(),UsuarioModificacion = @UsuarioModificacion
                                            where IdStatusCuenta = @IdStatusCuenta", statusCuentum);
        }

        public async Task Borrar(int idStatusCuenta)
        {
            using var connectio = new SqlConnection(connectionString);
            await connectio.ExecuteAsync("delete from STATUS_CUENTA where IdStatusCuenta = @idStatusCuenta", new { idStatusCuenta });
        }

        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from STATUS_CUENTA where Nombre = @Nombre;",
                                                                        new { nombre });
            return existe == 1;
        }
    }
}
