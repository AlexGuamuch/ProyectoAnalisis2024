using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryTipoSaldoCuentum
    {
        Task<IEnumerable<TipoSaldoCuentum>> ObtenerTodos();
        Task<TipoSaldoCuentum> ObtenerPorId(int idTipoSaldoCuenta);
        Task Crear(TipoSaldoCuentum tipoSaldoCuentum);
        Task Actualizar(TipoSaldoCuentum tipoSaldoCuentum);
        Task Borrar(int idTipoSaldoCuenta);
        Task<bool> Existe(string nombre);
    }

    public class RepositoryTipoSaldoCuentum : IRepositoryTipoSaldoCuentum
    {
        private readonly string connectionString;

        public RepositoryTipoSaldoCuentum(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoSaldoCuentum tipoSaldoCuentum)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("sptiposaldoinserta",
                                                             new
                                                             {
                                                                 Nombre = tipoSaldoCuentum.Nombre,
                                                                 UsuarioCreacion = tipoSaldoCuentum.UsuarioCreacion
                                                             },
                                                             commandType: System.Data.CommandType.StoredProcedure);
            tipoSaldoCuentum.IdTipoSaldoCuenta = id;
        }


        public async Task<IEnumerable<TipoSaldoCuentum>> ObtenerTodos()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoSaldoCuentum>(@"
                SELECT IdTipoSaldoCuenta, Nombre,FechaCreacion FROM TIPO_SALDO_CUENTA;");
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

        public async Task Actualizar(TipoSaldoCuentum tipoSaldoCuentum)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update TIPO_SALDO_CUENTA
                                            set Nombre = @Nombre, FechaModificacion=GETDATE(),UsuarioModificacion=@UsuarioModificacion
                                            where IdTipoSaldoCuenta=@IdTipoSaldoCuenta", tipoSaldoCuentum);
        }

        public async Task Borrar(int idTipoSaldoCuenta)
        {
            using var connectio = new SqlConnection(connectionString);
            await connectio.ExecuteAsync("delete from TIPO_SALDO_CUENTA where IdTipoSaldoCuenta = @idTipoSaldoCuenta", new { idTipoSaldoCuenta });
        }

        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from TIPO_SALDO_CUENTA where Nombre = @Nombre;",
                                                                        new { nombre });
            return existe == 1;
        }
    }
}
