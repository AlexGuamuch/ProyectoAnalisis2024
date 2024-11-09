using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryMenu
    {
        Task ActualizarGeneral(Menu menu);
        Task Borrar(int idMenu);
        Task Crear(Menu menu);
        Task<bool> Existe(string nombre);
        Task<IEnumerable<Menu>> Obtener();
        Task<Menu> ObtenerPorId(int idMenu);
    }
    public class RepositoryMenu: IRepositoryMenu
    {
        public readonly string connectionString;
        public RepositoryMenu(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<Menu>> Obtener() 
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Menu>("select IdMenu, IdModulo, Nombre, OrdenMenu from MENU");
        }

        public async Task Crear(Menu menu)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into 
                                                           MENU(IdModulo, Nombre, OrdenMenu, FechaCreacion, UsuarioCreacion) 
                                                           values(@IdModulo, @Nombre, @OrdenMenu, GETDATE(), @UsuarioCreacion);
                                                                select SCOPE_IDENTITY();", menu);
            menu.IdModulo = id;
        }

        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from MENU where Nombre = @Nombre;",
                                                                        new { nombre });
            return existe == 1;
        }

        public async Task<Menu> ObtenerPorId(int idMenu)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Menu>(@"select IdMenu, IdModulo, Nombre, OrdenMenu  
                                                                        from MENU where IdMenu = @IdMenu", new { idMenu });
        }

        public async Task Borrar(int idMenu)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("delete from MENU where IdMenu = @IdMenu", new { idMenu });
        }

        public async Task ActualizarGeneral(Menu menu)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update MENU
											set Nombre = @Nombre, OrdenMenu = @OrdenMenu,
											FechaModificacion = GETDATE(), UsuarioModificacion = @UsuarioModificacion
											where IdMenu = @IdMenu", menu);
        }

    }

}
