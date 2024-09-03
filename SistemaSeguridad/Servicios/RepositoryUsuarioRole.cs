using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryUsuarioRole
    {
		Task<string> Crear(UsuarioRole usuarioRole);
		Task<IEnumerable<UsuarioRole>> Obtener();
    }
    public class RepositoryUsuarioRole: IRepositoryUsuarioRole
    {
        private readonly string connectionString;

        public RepositoryUsuarioRole(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<UsuarioRole>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<UsuarioRole>(@"select IdUsuario, IdRole from USUARIO_ROLE");
        }

		public async Task<string> Crear(UsuarioRole usuarioRole)
		{
			using var connection = new SqlConnection(connectionString);
			var IdUsuario = await connection.QuerySingleAsync<string>(@"insert into USUARIO_ROLE(IdUsuario, IdRole,FechaCreacion,UsuarioCreacion)
                                                                values(@IdUsuario, @IdRole,  GETDATE(), @UsuarioCreacion);
                                                                select SCOPE_IDENTITY();", usuarioRole);
			return IdUsuario;
		}
	}
}
