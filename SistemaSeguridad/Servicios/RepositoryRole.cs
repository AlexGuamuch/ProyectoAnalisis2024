using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryRole
    {
		Task ActualizarGeneral(Role role);
		Task Borrar(int idRole);
		Task Crear(Role role);
		Task<bool> Existe(string nombre);
		Task<IEnumerable<Role>> Obtener();
		Task<Role> ObtenerPorId(int role);
	}
    public class RepositoryRole: IRepositoryRole
    {
        private readonly string connectionString;

        public RepositoryRole(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

		public async Task Crear(Role role)
		{
			using var connection = new SqlConnection(connectionString);

			var id = await connection.QuerySingleAsync<int>(@"insert into [ROLE](Nombre,FechaCreacion,UsuarioCreacion)
                                                                values(@Nombre,GETDATE(),@UsuarioCreacion);
                                                                select SCOPE_IDENTITY();", role);
			role.IdRole= id;
		}

		public async Task<bool> Existe(string nombre)
		{
			using var connection = new SqlConnection(connectionString);
			var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from [ROLE] where Nombre = @Nombre;",
																		new { nombre });
			return existe == 1;
		}

		public async Task<IEnumerable<Role>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Role>(@"select IdRole, Nombre from ROLE");
        }

		public async Task<Role> ObtenerPorId(int idRole)
		{
			using var connection = new SqlConnection(connectionString);
			return await connection.QueryFirstOrDefaultAsync<Role>(@"select IdRole, Nombre  
                                                                        from [ROLE] where IdRole = @IdRole", new { idRole });
		}

		public async Task Borrar(int idRole)
		{
			using var connection = new SqlConnection(connectionString);
			await connection.ExecuteAsync("delete from [ROLE] where IdRole = @IdRole", new { idRole});
		}

		public async Task ActualizarGeneral(Role role)
		{
			using var connection = new SqlConnection(connectionString);
			await connection.ExecuteAsync(@"update [ROLE]
											set Nombre = @Nombre,
											FechaModificacion = GETDATE(), UsuarioModificacion = @UsuarioModificacion
											where IdRole = @IdRole", role);
		}
	}
}
