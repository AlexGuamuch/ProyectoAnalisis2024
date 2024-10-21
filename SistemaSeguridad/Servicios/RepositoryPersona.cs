using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryPersona
    {
        Task Crear(Persona persona);
        Task<IEnumerable<Persona>> Obtener();
        Task<Persona> ObtenerPorId(int idPersona);
        Task Actualizar(Persona persona);
        Task Borrar(int idPersona);
        Task<IEnumerable<PersonaViewModel>> ObtenerClientes();
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
            var sql = @"
                INSERT INTO PERSONA (Nombre, Apellido, FechaNacimiento, IdGenero, 
                                     Direccion, Telefono, CorreoElectronico, 
                                     IdEstadoCivil, FechaCreacion, UsuarioCreacion)
                VALUES (@Nombre, @Apellido, @FechaNacimiento, @IdGenero, 
                        @Direccion, @Telefono, @CorreoElectronico, 
                        @IdEstadoCivil, GETDATE(), @UsuarioCreacion)";

            var parameters = new
            {
                persona.Nombre,
                persona.Apellido,
                persona.FechaNacimiento, // Asegúrate de que esto sea DateTime
                persona.IdGenero,
                persona.Direccion,
                persona.Telefono,
                persona.CorreoElectronico,
                persona.IdEstadoCivil,
                persona.UsuarioCreacion
            };

            await connection.ExecuteAsync(sql, parameters);
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
            var sql = @"
                UPDATE PERSONA
                SET Nombre = @Nombre,
                    Apellido = @Apellido,
                    FechaNacimiento = @FechaNacimiento,
                    IdGenero = @IdGenero,
                    Direccion = @Direccion,
                    Telefono = @Telefono,
                    CorreoElectronico = @CorreoElectronico,
                    IdEstadoCivil = @IdEstadoCivil,
                    FechaModificacion = GETDATE(),
                    UsuarioModificacion = @UsuarioModificacion
                WHERE IdPersona = @IdPersona;";

            var parameters = new
            {
                persona.IdPersona,
                persona.Nombre,
                persona.Apellido,
                persona.FechaNacimiento, // Asegúrate de que esto sea DateTime
                persona.IdGenero,
                persona.Direccion,
                persona.Telefono,
                persona.CorreoElectronico,
                persona.IdEstadoCivil,
                persona.UsuarioModificacion
            };

            await connection.ExecuteAsync(sql, parameters);
        }

        public async Task Borrar(int idPersona)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                DELETE FROM PERSONA 
                WHERE IdPersona = @IdPersona;",
                new { IdPersona = idPersona });
        }
        
        public async Task<IEnumerable<PersonaViewModel>> ObtenerClientes()
        {
        using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<PersonaViewModel>(
            @"SELECT IdPersona, CONCAT(Nombre, ' ', Apellido) AS NombreCompleto 
              FROM Persona");
        }
    }
    
    public class PersonaViewModel
    {
        public int IdPersona { get; set; }
        public string NombreCompleto { get; set; }
    }
}
