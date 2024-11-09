using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
	public interface IRepositoyEmpresa
	{
        Task ActualizarGeneral(Empresa empresa);
        Task Borrar(int idEmpresa);
        Task Crear(Empresa empresa);
		Task<bool> Existe(string nombre);
		Task<IEnumerable<Empresa>> Obtener();
		Task<Empresa> ObtenerPasswordRules();
		Task<Empresa> ObtenerPorId(int idEmpresa);
	}
	public class RepositoryEmpresa: IRepositoyEmpresa
	{
		private readonly string connectionString;

        public RepositoryEmpresa(IConfiguration configuration)
		{
			connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		public async Task Crear(Empresa empresa)
		{
			using var connection = new SqlConnection(connectionString);
			var id = await connection.QuerySingleAsync<int>("spempresainserta",
															 new
															 { 
																Nombre = empresa.Nombre, Direccion = empresa.Direccion, Nit = empresa.Nit,
																PasswordCantidadMayusculas = empresa.PasswordCantidadMayusculas,
																PasswordCantidadMinusculas = empresa.PasswordCantidadMinusculas,
																PasswordCantidadCaracteresEspeciales = empresa.PasswordCantidadCaracteresEspeciales,
																PasswordCantidadCaducidadDias = empresa.PasswordCantidadCaducidadDias,
																PasswordLargo = empresa.PasswordLargo,
																PasswordIntentosAntesDeBloquear = empresa.PasswordIntentosAntesDeBloquear,
																PasswordCantidadNumeros = empresa.PasswordCantidadNumeros,
																PasswordCantidadPreguntasValidar = empresa.PasswordCantidadPreguntasValidar,
																UsuarioCreacion = empresa.UsuarioCreacion
															 },
															 commandType: System.Data.CommandType.StoredProcedure);
			
			/*var id = await connection.QuerySingleAsync<int>(@"insert into EMPRESA(Nombre, Direccion, Nit,FechaCreacion,UsuarioCreacion)
															values(@Nombre,@Direccion,@Nit,GETDATE(),@UsuarioCreacion);
															select SCOPE_IDENTITY();", empresa);
			*/
			empresa.IdEmpresa = id;
		}

		public async Task<bool> Existe(string nombre)
		{
			using var connection = new SqlConnection(connectionString);
			var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from EMPRESA where Nombre = @Nombre;",
																		new { nombre });
			return existe == 1;
		}

		public async Task<IEnumerable<Empresa>> Obtener()
		{
			using var connection = new SqlConnection(connectionString);
			return await connection.QueryAsync<Empresa>(@"select IdEmpresa, Nombre, Direccion, Nit from EMPRESA");
		}

		public async Task<Empresa> ObtenerPorId(int idEmpresa)
		{
			using var connection = new SqlConnection(connectionString);
			return await connection.QueryFirstOrDefaultAsync<Empresa>(@"select IdEmpresa, Nombre, Direccion, Nit  
                                                                        from EMPRESA where IdEmpresa = @IdEmpresa", new { idEmpresa});
		}

		public async Task Borrar(int idEmpresa)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("delete from Empresa where IdEmpresa = @IdEmpresa", new { idEmpresa });
        }

        public async Task ActualizarGeneral(Empresa empresa)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update EMPRESA
											set Nombre = @Nombre, Direccion = @Direccion, Nit = @Nit, 
											FechaModificacion = GETDATE(), UsuarioModificacion = @UsuarioModificacion
											where IdEmpresa = @IdEmpresa", empresa);
        }

		public async Task<Empresa> ObtenerPasswordRules()
		{
			using var connection = new SqlConnection(connectionString);
			var query = "select PasswordLargo, PasswordCantidadMayusculas, PasswordCantidadMinusculas, PasswordCantidadNumeros, PasswordCantidadCaracteresEspeciales from EMPRESA where IdEmpresa = 1;";
			return await connection.QuerySingleOrDefaultAsync<Empresa>(query);
		}
	}
}
