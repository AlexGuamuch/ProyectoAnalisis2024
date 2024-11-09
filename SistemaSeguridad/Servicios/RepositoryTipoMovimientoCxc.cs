using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryTipoMovimientoCxc
    {
        Task<IEnumerable<TipoMovimientoCxc>> ObtenerTodos();
        Task<TipoMovimientoCxc> ObtenerPorId(int idTipoMovimientoCxc);
        Task Crear(TipoMovimientoCxc tipoMovimiento);
        Task Actualizar(TipoMovimientoCxc tipoMovimientoCxc);
        Task Borrar(int idTipoMovimientoCxc);
        Task<bool> Existe(string nombre);
    }

    public class RepositoryTipoMovimientoCxc : IRepositoryTipoMovimientoCxc
    {
        private readonly string connectionString;

        public RepositoryTipoMovimientoCxc(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoMovimientoCxc tipoMovimiento)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("sprtipomovimientoinserta",
                                                             new
                                                             {
                                                                 Nombre = tipoMovimiento.Nombre,
                                                                 OperacionCuentaCorriente = tipoMovimiento.OperacionCuentaCorriente,
                                                                 UsuarioCreacion = tipoMovimiento.UsuarioCreacion
                                                             },
                                                             commandType: System.Data.CommandType.StoredProcedure);
            tipoMovimiento.IdTipoMovimientoCxc= id;
        }


        public async Task<IEnumerable<TipoMovimientoCxc>> ObtenerTodos()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoMovimientoCxc>(@"
                SELECT IdTipoMovimientoCxc, Nombre, OperacionCuentaCorriente, FechaCreacion FROM TIPO_MOVIMIENTO_CXC;");
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

        public async Task Actualizar(TipoMovimientoCxc tipoMovimientoCxc)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update TIPO_MOVIMIENTO_CXC
                                          set Nombre = @Nombre,FechaModificacion=GETDATE(),OperacionCuentaCorriente=@OperacionCuentaCorriente,
                                          UsuarioModificacion=@UsuarioModificacion
                                          where IdTipoMovimientoCXC = @IdTipoMovimientoCXC", tipoMovimientoCxc);      
        }

        public async Task Borrar(int idTipoMovimientoCxc)
        {
            using var connectio = new SqlConnection(connectionString);
            await connectio.ExecuteAsync("delete from TIPO_MOVIMIENTO_CXC where IdTipoMovimientoCXC = @idTipoMovimientoCxc", new { idTipoMovimientoCxc });
        }

        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from TIPO_MOVIMIENTO_CXC where Nombre = @Nombre;",
                                                                        new { nombre });
            return existe == 1;
        }

    }
}
