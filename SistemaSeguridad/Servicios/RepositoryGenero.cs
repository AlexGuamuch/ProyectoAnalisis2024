using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SistemaSeguridad.Servicios
{
    //Se crea una interfaz para acceder a la informacion en el Controller
    public interface IRepositoryGenero
    {
        Task Actualizar(Genero genero);
        Task Borrar(int idGenero);
        Task Crear(Genero genero);
        Task<bool> Existe(string nombre);
        Task<IEnumerable<Genero>> Obtener();
        Task<Genero> ObtenerPorId(int idgenero);
    }
    public class RepositoryGenero: IRepositoryGenero
    {
        //Aqui se hace inicializa la variable de conexion
        private readonly string connectionString;

        //Este es el construcctor de la clase donde accede a la conexion
        public RepositoryGenero(IConfiguration configuration)
        {
            //en esta variable se hace la conexion a la base de datos esta variable se encuentra en el appsettings.json
            //"DefaultConnection" esta es la variable de conexion
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //Este es el metodo que inserta la base de datos
        public async Task Crear(Genero genero) 
        { 
            //Se usa la variable conexion
            using var connection = new SqlConnection(connectionString);
            //Aqui se inserta el query por el momento es todo eso pero se crearan procedimientos almacenados en SQl
            //para no estar escribiendo querys aqui y sera mas practico
            var id = await connection.QuerySingleAsync<int>("spgeneroinserta",
                                                             new { Nombre = genero.Nombre,
                                                             UsuarioCreacion = genero.UsuarioCreacion}, 
                                                             commandType: System.Data.CommandType.StoredProcedure);
            genero.IdGenero = id;
        }

        //Metodo que hace la validacion si existe un registro
        public async Task<bool> Existe(string nombre) 
        {
            //Se usa la variable conexion
            using var connection = new SqlConnection(connectionString);
            //La validacion no es complicada solo hace una busqueda la base de datos y si existe el registro muestra 1
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"select 1 from GENERO where Nombre = @Nombre;",
                                                                        new {nombre});
            //Si el query retorna 1 existe el registro
            return existe == 1;
        }

        //Este metodo retorna el listado de todos los registros
        public async Task<IEnumerable<Genero>> Obtener() 
        {
            //Se usa la variable conexion
            using var connection = new SqlConnection(connectionString);
            //Retorna todos los registros buscados
            return await connection.QueryAsync<Genero>(@"select IdGenero, Nombre, FechaCreacion from GENERO");
        }

        //Este metodo es el de acctualizar el registro
        public async Task Actualizar(Genero genero) 
        {
            //Se usa la variable conexion
            using var connection = new SqlConnection(connectionString);
            //ExecuteAsync permite ejecutar un query que no va a retornar nada, seria un update
            //Lo que hace solo de acctualizar el registro con el id
            await connection.ExecuteAsync(@"update GENERO set Nombre = @Nombre,
                                          FechaModificacion = GETDATE(), UsuarioModificacion = @UsuarioModificacion
                                          where IdGenero = @IdGenero", genero);
        }

        //Se busca el registro por ID
        public async Task<Genero> ObtenerPorId(int idgenero)
        {
            //Se usa la variable conexion
            using var connection = new SqlConnection(connectionString);
            //Retorna el registro encontrado con el id para hacer el update
            return await connection.QueryFirstOrDefaultAsync<Genero>(@"select IdGenero, Nombre, FechaCreacion,UsuarioCreacion  
                                                                        from GENERO where IdGenero = @idgenero", new {idgenero});
        }

        public async Task Borrar(int idGenero) 
        { 
            using var connectio = new SqlConnection(connectionString);
            await connectio.ExecuteAsync("delete from GENERO where IdGenero = @IdGenero", new { idGenero });
        }
    }

}
