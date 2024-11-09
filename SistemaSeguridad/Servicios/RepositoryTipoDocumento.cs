using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryTipoDocumento
    {
        Task<IEnumerable<TipoDocumento>> ObtenerTodos();
        Task<TipoDocumento> ObtenerPorId(int idTipoDocumento);
        Task Crear(TipoDocumento tipoDocumento);
        Task Actualizar(TipoDocumento tipoDocumento);
        Task Borrar(int idTipoDocumento);
        Task<bool> Existe(string nombre);
    }

    public class RepositoryTipoDocumento : IRepositoryTipoDocumento
    {
        private readonly string connectionString;

        public RepositoryTipoDocumento(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoDocumento tipoDocumento)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("sptipodocumentoinserta",
                                                             new
                                                             {
                                                                 Nombre = tipoDocumento.Nombre,
                                                                 UsuarioCreacion = tipoDocumento.UsuarioCreacion
                                                             },
                                                             commandType: System.Data.CommandType.StoredProcedure);
            tipoDocumento.IdTipoDocumento= id;
        }


        public async Task<IEnumerable<TipoDocumento>> ObtenerTodos()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoDocumento>(@"
                SELECT IdTipoDocumento, Nombre, FechaCreacion FROM TIPO_DOCUMENTO;");
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

        public async Task Actualizar(TipoDocumento tipoDocumento)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update TIPO_DOCUMENTO
                                            set Nombre = @Nombre, FechaModificacion = GETDATE(),UsuarioModificacion = @UsuarioModificacion
                                            where IdTipoDocumento = @IdTipoDocumento", tipoDocumento);
        }

        public async Task Borrar(int idTipoDocumento)
        {
            using var connectio = new SqlConnection(connectionString);
            await connectio.ExecuteAsync("delete from TIPO_DOCUMENTO where IdTipoDocumento = @idTipoDocumento", new { idTipoDocumento });
        }

        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from TIPO_DOCUMENTO where Nombre = @Nombre;",
                                                                        new { nombre });
            return existe == 1;
        }
    }
}
