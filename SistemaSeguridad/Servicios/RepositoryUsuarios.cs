using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
	public interface IRepositoryUsuarios
	{
		Task<UsuarioPrueba> BuscarUsuarioEmail(string CorreoElectronico);
		Task<UsuarioPrueba> BuscarUsuarioNombre(string Nombre);
		Task<string> CrearUsuario(UsuarioPrueba usuarioPrueba);
		Task<IEnumerable<UsuarioPrueba>> Obtener();
	}
	public class RepositoryUsuarios: IRepositoryUsuarios
	{
		private readonly string connectionString;
		public RepositoryUsuarios(IConfiguration configuration) 
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public async Task<string> CrearUsuario(UsuarioPrueba usuarioPrueba) 
		{
			using var connection = new SqlConnection(connectionString);
			var IdUsuario = await connection.QuerySingleAsync<string>(@"insert into USUARIO(IdUsuario, 
								CorreoElectronico,Nombre,Apellido,FechaNacimiento,IdStatusUsuario,Password,TelefonoMovil,
								IdGenero,IntentosDeAcceso,RequiereCambiarPassword,IdSucursal,FechaCreacion,UsuarioCreacion)
								values(@IdUsuario,@CorreoElectronico,@Nombre,@Apellido,@FechaNacimiento,1,@Password,@TelefonoMovil,
								@IdGenero,0,1,@IdSucursal,GETDATE(),@UsuarioCreacion);
								SELECT IdUsuario FROM USUARIO WHERE IdUsuario = @IdUsuario;", usuarioPrueba);
			return IdUsuario;
		}

		public async Task<UsuarioPrueba> BuscarUsuarioEmail(string CorreoElectronico) 
		{
			using var connection = new SqlConnection(connectionString);
			return await connection.QuerySingleOrDefaultAsync<UsuarioPrueba>
				("select * from USUARIO where CorreoElectronico = @CorreoElectronico", new { CorreoElectronico });
		}

		public async Task<UsuarioPrueba> BuscarUsuarioNombre(string Nombre)
		{
			using var connection = new SqlConnection(connectionString);
			return await connection.QuerySingleOrDefaultAsync<UsuarioPrueba>
				("select * from USUARIO where IdUsuario = @Nombre", new { Nombre });
		}

		public async Task<IEnumerable<UsuarioPrueba>> Obtener()
		{
			using var connection = new SqlConnection(connectionString);
			return await connection.QueryAsync<UsuarioPrueba>(@"select IdUsuario, Nombre from USUARIO");
		}
	}
}
