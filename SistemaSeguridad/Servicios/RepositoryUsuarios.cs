using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
	public interface IRepositoryUsuarios
	{
		Task Actualizar(UsuarioPrueba usuario);
        	Task Borrar(string idUsuario);
        	Task<UsuarioPrueba> BuscarUsuarioEmail(string CorreoElectronico);
		Task<UsuarioPrueba> BuscarUsuario(string Nombre);
		Task<UsuarioPrueba> BuscarUsuarioNombre(string Nombre);
		Task<string> CrearUsuario(UsuarioPrueba usuarioPrueba);
        	Task<bool> Existe(string IdUsuario);
        	Task<IEnumerable<UsuarioPrueba>> Obtener();
		Task<UsuarioPrueba> ObtenerPorId(string IdUsuario);
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
			return await connection.QueryAsync<UsuarioPrueba>(@"select IdUsuario, Nombre, Apellido, FechaNacimiento, CorreoElectronico from USUARIO");
		}

        public async Task<bool> Existe(string IdUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from USUARIO where IdUsuario = @IdUsuario;",
                                                                        new { IdUsuario });
            return existe == 1;
        }

        public async Task Borrar(string idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("delete from USUARIO where IdUsuario = @IdUsuario", new { idUsuario });
        }

 	public async Task<UsuarioPrueba> ObtenerPorId( string IdUsuario) //Servicio para selccionar los datos a editar del usuario 
	{
	using var connection = new SqlConnection(connectionString);
	return await connection.QueryFirstOrDefaultAsync<UsuarioPrueba>(
        @"SELECT Nombre, Apellido, FechaNacimiento, CorreoElectronico, TelefonoMovil
			FROM Usuario 
			WHERE IdUsuario = @IdUsuario", new { IdUsuario }
		);
	}

	public async Task Actualizar(UsuarioPrueba usuario)
	{
	using var connection = new SqlConnection(connectionString);
	await connection.ExecuteAsync(@"UPDATE Usuario 
										SET Nombre = @Nombre, Apellido = @Apellido, FechaNacimiento = @FechaNacimiento, 
										CorreoElectronico = @CorreoElectronico, TelefonoMovil = @TelefonoMovil
										WHERE IdUsuario = @IdUsuario", usuario
										);
	}

 	public async Task<UsuarioPrueba> BuscarUsuario(string nombre)
	{
	using var connection = new SqlConnection(connectionString);
	string query = "SELECT * FROM USUARIO WHERE Nombre = @Nombre";
	return await connection.QuerySingleOrDefaultAsync<UsuarioPrueba>(query, new { Nombre = nombre });
	}
    }
}
