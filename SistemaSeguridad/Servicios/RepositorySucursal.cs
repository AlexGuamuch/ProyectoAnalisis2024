using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
	public interface IReposirorySucursal
	{
		Task ActualizarGeneral(Sucursal sucursal);
		Task Borrar(int idSucursal);
		Task Crear(Sucursal sucursal);
		Task<bool> Existe(string nombre);
		Task<IEnumerable<Sucursal>> Obtener();
        Task<Sucursal> ObtenerPorId(int idSucursal);
    }
	public class RepositorySucursal: IReposirorySucursal
    {
        private readonly string connectionString;
        public RepositorySucursal(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Sucursal sucursal)
        {   
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("spsucursalinserta",
                                                             new
                                                             {
                                                                 Nombre = sucursal.Nombre,
                                                                 Direccion = sucursal.Direccion,
                                                                 IdEmpresa = sucursal.IdEmpresa,
                                                                 UsuarioCreacion = sucursal.UsuarioCreacion
                                                             },
                                                             commandType: System.Data.CommandType.StoredProcedure);
            sucursal.IdSucursal = id;
        }

        public async Task<bool> Existe(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from SUCURSAL where Nombre = @Nombre;",
                                                                        new { nombre });
            return existe == 1;
        }

		public async Task<IEnumerable<Sucursal>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Sucursal>(@"select IdSucursal, Nombre, Direccion from SUCURSAL");
        }

        public async Task<Sucursal> ObtenerPorId(int idSucursal)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Sucursal>(@"select IdSucursal, Nombre, Direccion, IdEmpresa  
                                                                        from SUCURSAL where IdSucursal = @IdSucursal", new { idSucursal });
        }

        public async Task Borrar(int idSucursal)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("delete from SUCURSAL where IdSucursal = @IdSucursal", new { idSucursal });
        }

        public async Task ActualizarGeneral(Sucursal sucursal)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update SUCURSAL
                                            set Nombre = @Nombre, Direccion = @Direccion, FechaModificacion = GETDATE(),
                                            UsuarioModificacion = @UsuarioModificacion
                                            where IdSucursal = @IdSucursal
                                            ", sucursal);
        }
    }
}
