using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryEstadoCivil
    {
        Task<IEnumerable<EstadoCivil>> ObtenerTodos();
        Task<EstadoCivil> ObtenerPorId(int id);
        Task Crear(EstadoCivil estado);
        Task Actualizar(EstadoCivil estado);
        Task Borrar(int idEstadoCivil);
        Task<bool> Existe(string nombre);
        Task<IEnumerable<EstadoCivil>> Obtener();
    }

    public class RepositoryEstadoCivil : IRepositoryEstadoCivil
    {
        private readonly string connectionString;

        public RepositoryEstadoCivil(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(EstadoCivil estado)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("spestadocivilinserta",
                                                             new
                                                             {
                                                                 Nombre = estado.Nombre,
                                                                 UsuarioCreacion = estado.UsuarioCreacion
                                                             },
                                                             commandType: System.Data.CommandType.StoredProcedure);
            estado.IdEstadoCivil = id;
        }

        public async Task<IEnumerable<EstadoCivil>> ObtenerTodos()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<EstadoCivil>(@"SELECT IdEstadoCivil, Nombre, FechaCreacion FROM ESTADO_CIVIL;");
        }

        public async Task<EstadoCivil> ObtenerPorId(int id)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<EstadoCivil>(@"
                SELECT IdEstadoCivil, Nombre, FechaCreacion, UsuarioCreacion, 
                       FechaModificacion, UsuarioModificacion
                FROM ESTADO_CIVIL
                WHERE IdEstadoCivil = @Id;",
                new { Id = id });
        }

        public async Task Actualizar(EstadoCivil estado)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update ESTADO_CIVIL
                                            set Nombre = @Nombre,FechaModificacion = GETDATE(),UsuarioModificacion = @UsuarioModificacion
                                            where IdEstadoCivil = @IdEstadoCivil", estado);
        }

        public async Task Borrar(int idEstadoCivil)
        {
            using var connectio = new SqlConnection(connectionString);
            await connectio.ExecuteAsync("delete from ESTADO_CIVIL where IdEstadoCivil = @idEstadoCivil", new { idEstadoCivil });
        }


        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from ESTADO_CIVIL where Nombre = @Nombre;",
                                                                        new { nombre });
            return existe == 1;
        }


        public async Task<IEnumerable<EstadoCivil>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<EstadoCivil>(@"select IdEstadoCivil,Nombre,FechaCreacion from ESTADO_CIVIL");
        }
    }
}
