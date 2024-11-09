using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
	public interface IRepositoryStatusUsuario
	{
		Task ActualizarGeneral(StatusUsuario statusUsuario);
		Task Borrar(int idStatusUsuario);
		Task Crear(StatusUsuario statusUsuario);
		Task<bool> Existe(string nombre);
		Task<IEnumerable<StatusUsuario>> Obtener();
		Task<StatusUsuario> ObtenerPorId(int idStatusUsuario);
	}
	public class RepositoryStatusUsuario: IRepositoryStatusUsuario
	{
		private readonly string connectionString;

        public RepositoryStatusUsuario(IConfiguration configuration)
        {
			connectionString = configuration.GetConnectionString("DefaultConnection");
        }

		public async Task Crear(StatusUsuario statusUsuario) 
		{
			using var connection = new SqlConnection(connectionString);

			var id = await connection.QuerySingleAsync<int>(@"insert into STATUS_USUARIO(Nombre,FechaCreacion,UsuarioCreacion)
														values(@Nombre,GETDATE(),@UsuarioCreacion);
														select SCOPE_IDENTITY();",statusUsuario);
			statusUsuario.IdStatusUsuario= id;
		}

		public async Task<bool> Existe(string nombre)
		{
			using var connection = new SqlConnection(connectionString);
			var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from STATUS_USUARIO where Nombre = @Nombre;",
																		new { nombre });
			return existe == 1;
		}

		public async Task<IEnumerable<StatusUsuario>> Obtener()
		{
			using var connection = new SqlConnection(connectionString);
			return await connection.QueryAsync<StatusUsuario>(@"select IdStatusUsuario, Nombre from STATUS_USUARIO");
		}

		public async Task<StatusUsuario> ObtenerPorId(int idStatusUsuario)
		{
			using var connection = new SqlConnection(connectionString);
			return await connection.QueryFirstOrDefaultAsync<StatusUsuario>(@"select IdStatusUsuario, Nombre  
                                                                        from STATUS_USUARIO where IdStatusUsuario = @IdStatusUsuario", new { idStatusUsuario });
		}

		public async Task Borrar(int idStatusUsuario)
		{
			using var connection = new SqlConnection(connectionString);
			await connection.ExecuteAsync("delete from STATUS_USUARIO where IdStatusUsuario = @IdStatusUsuario", new { idStatusUsuario });
		}

		public async Task ActualizarGeneral(StatusUsuario statusUsuario)
		{
			using var connection = new SqlConnection(connectionString);
			await connection.ExecuteAsync(@"update STATUS_USUARIO
											set Nombre = @Nombre,
											FechaModificacion = GETDATE(), UsuarioModificacion = @UsuarioModificacion
											where IdStatusUsuario = @IdStatusUsuario", statusUsuario);
		}

	}
}
