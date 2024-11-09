using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryOpcion
    {
        Task ActualizarGeneral(Opcion opcion);
        Task Borrar(int idOpcion);
        Task Crear(Opcion opcion);
        Task<bool> Existe(string nombre);
        Task<IEnumerable<Opcion>> Obtener();
        Task<Opcion> ObtenerPorId(int idOpcion);
    }
    public class RepositoryOpcion: IRepositoryOpcion
    {
        public readonly string connectionString;
        public RepositoryOpcion(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Opcion>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Opcion>("select IdOpcion, IdMenu, Nombre, OrdenMenu from OPCION");
        }

        public async Task Crear(Opcion opcion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into 
                                                            OPCION(IdMenu, Nombre, OrdenMenu, Pagina, FechaCreacion, UsuarioCreacion) 
                                                            values(@IdMenu, @Nombre, @OrdenMenu, @Pagina, GETDATE(), @UsuarioCreacion)
                                                            select SCOPE_IDENTITY();", opcion);
            opcion.IdOpcion = id;
        }

        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from OPCION where Nombre = @Nombre;",
                                                                        new { nombre });
            return existe == 1;
        }

        public async Task<Opcion> ObtenerPorId(int idOpcion)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Opcion>(@"select IdOpcion, IdMenu, Nombre, OrdenMenu, Pagina  
                                                                        from OPCION where IdOpcion = @IdOpcion", new { idOpcion});
        }

        public async Task Borrar(int idOpcion)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("delete from OPCION where IdOpcion = @IdOpcion", new { idOpcion });
        }

        public async Task ActualizarGeneral(Opcion opcion)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update OPCION
											set Nombre = @Nombre, OrdenMenu = @OrdenMenu,Pagina = @Pagina, 
											FechaModificacion = GETDATE(), UsuarioModificacion = @UsuarioModificacion
											where IdOpcion = @IdOpcion", opcion);
        }
    }
}
