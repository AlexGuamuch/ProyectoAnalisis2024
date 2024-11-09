using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryModulo
    {
        Task ActualizarGeneral(Modulo modulo);
        Task Borrar(int idModulo);
        Task Crear(Modulo modulo);
        Task<bool> Existe(string nombre);
        Task<IEnumerable<Modulo>> Obtener();
        Task<Modulo> ObtenerPorId(int idModulo);
    }
    public class RepositoryModulo: IRepositoryModulo
    {
        private readonly string connectionString;

        public RepositoryModulo(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Modulo>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Modulo>(@"select IdModulo, Nombre from MODULO");
        }

        public async Task Crear(Modulo modulo) 
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into MODULO(Nombre,OrdenMenu,FechaCreacion,UsuarioCreacion)
                                                                values(@Nombre,@OrdenMenu,GETDATE(),@UsuarioCreacion);
                                                                select SCOPE_IDENTITY();", modulo);
            modulo.IdModulo = id;
        }

        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from MODULO where Nombre = @Nombre;",
                                                                        new { nombre });
            return existe == 1;
        }

        public async Task<Modulo> ObtenerPorId(int idModulo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Modulo>(@"select IdModulo, Nombre, OrdenMenu  
                                                                        from MODULO where IdModulo = @IdModulo", new { idModulo});
        }

        public async Task Borrar(int idModulo)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("delete from MODULO where IdModulo = @IdModulo", new { idModulo });
        }

        public async Task ActualizarGeneral(Modulo modulo)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update MODULO
											set Nombre = @Nombre, OrdenMenu = @OrdenMenu,
											FechaModificacion = GETDATE(), UsuarioModificacion = @UsuarioModificacion
											where IdModulo = @IdModulo", modulo);
        }
    }
}
