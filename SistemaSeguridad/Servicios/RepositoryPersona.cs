using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryPersona
    {
        Task Actualizar(Persona persona);
        Task Borrar(int idPersona);
        Task<IEnumerable<Persona>> BuscarPorDpi(string noDocumento);
        Task<IEnumerable<Persona>> BuscarPorNombre(string nombre);
        Task Crear(Persona persona);
        Task<IEnumerable<Persona>> Obtener();
        Task<IEnumerable<PersonaViewModel>> ObtenerClientes();
        Task<Persona> ObtenerPorId(int idPersona);
    }

    public class RepositoryPersona : IRepositoryPersona
    {
        private readonly string connectionString;

        public RepositoryPersona(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Persona persona)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into PERSONA(Nombre,Apellido,FechaNacimiento,IdGenero,Direccion,
					                                          Telefono,CorreoElectronico,IdEstadoCivil,FechaCreacion,UsuarioCreacion)
                                                              values(@Nombre,@Apellido,@FechaNacimiento,@IdGenero,@Direccion,@Telefono,
                                                              @CorreoElectronico,@IdEstadoCivil,GETDATE(),@UsuarioCreacion);
                                                              select SCOPE_IDENTITY();", persona);
            persona.IdPersona = id;
        }


        public async Task<IEnumerable<Persona>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Persona>(@"
                SELECT IdPersona, Nombre, Apellido, FechaNacimiento, IdGenero, 
                       Direccion, Telefono, CorreoElectronico, IdEstadoCivil, 
                       FechaCreacion, UsuarioCreacion, FechaModificacion, 
                       UsuarioModificacion
                FROM PERSONA;");
        }

        public async Task<Persona> ObtenerPorId(int idPersona)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Persona>(@"
                SELECT IdPersona, Nombre, Apellido, FechaNacimiento, IdGenero, 
                       Direccion, Telefono, CorreoElectronico, IdEstadoCivil, 
                       FechaCreacion, UsuarioCreacion, FechaModificacion, 
                       UsuarioModificacion
                FROM PERSONA
                WHERE IdPersona = @IdPersona;",
                new { IdPersona = idPersona });
        }


        public async Task Actualizar(Persona persona)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"update PERSONA set Nombre=@Nombre,Apellido=@Apellido,FechaModificacion=@FechaNacimiento,IdGenero=@IdGenero,Telefono=@Telefono,
                                            CorreoElectronico=@CorreoElectronico,IdEstadoCivil=@IdEstadoCivil,FechaModificacion=GETDATE(),UsuarioModificacion=@UsuarioModificacion
                                            where IdPersona=@IdPersona", persona);
        }

        public async Task<IEnumerable<PersonaViewModel>> ObtenerClientes()
        {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<PersonaViewModel>(
            @"SELECT IdPersona, CONCAT(Nombre, ' ', Apellido) AS NombreCompleto 
              FROM Persona");
        }

        public async Task Borrar(int idPersona)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("delete from PERSONA where IdPersona= @idPersona", new { idPersona });
        }

        public async Task<IEnumerable<Persona>> BuscarPorNombre(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var query = @"select * from PERSONA WHERE Nombre LIKE '%' + @Nombre + '%' or Apellido LIKE '%' + @Nombre + '%' ";
            return await connection.QueryAsync<Persona>(query, new { Nombre = nombre });
        }


        public async Task<IEnumerable<Persona>> BuscarPorDpi(string noDocumento)
        {
            using var connection = new SqlConnection(connectionString);
            var query = @"select p.* from PERSONA p inner join DOCUMENTO_PERSONA dp on p.IdPersona = dp.IdPersona where dp.NoDocumento = @NoDocumento";
            return await connection.QueryAsync<Persona>(query, new { NoDocumento = noDocumento });
        }

    }
    
    public class PersonaViewModel
    {
        public int IdPersona { get; set; }
        public string NombreCompleto { get; set; }
    }
}
