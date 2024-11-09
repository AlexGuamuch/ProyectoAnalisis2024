using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryDocumentoPersona
    {
        Task Crear(DocumentoPersona documentoPersona);
        Task<bool> Existe(int idTipoDocumento, int idPersona);
        Task<bool> ExisteNoDocumento(string noDocumento); // Nuevo método para verificar NoDocumento
        Task<IEnumerable<DocumentoPersona>> Obtener();
        Task<DocumentoPersona> ObtenerPorId(int idTipoDocumento, int idPersona);
        Task ActualizarGeneral(DocumentoPersona documentoPersona);
        Task Borrar(int idTipoDocumento, int idPersona);
    }

    public class RepositoryDocumentoPersona : IRepositoryDocumentoPersona
    {
        private readonly string connectionString;

        public RepositoryDocumentoPersona(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(DocumentoPersona documentoPersona)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                // Verificar si el NoDocumento ya existe
                if (await ExisteNoDocumento(documentoPersona.NoDocumento))
                {
                    throw new Exception("El NoDocumento ya existe."); // O maneja esto como desees
                }

                var sql = @"
                INSERT INTO DOCUMENTO_PERSONA (IdPersona, NoDocumento, IdTipoDocumento)
                VALUES (@IdPersona, @NoDocumento, @IdTipoDocumento)";

                var parameters = new
                {
                    IdPersona = documentoPersona.IdPersona,
                    NoDocumento = documentoPersona.NoDocumento,
                    IdTipoDocumento = documentoPersona.IdTipoDocumento
                };

                await connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<bool> Existe(int idTipoDocumento, int idPersona)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"
                SELECT 1 FROM DOCUMENTO_PERSONA 
                WHERE IdTipoDocumento = @IdTipoDocumento AND IdPersona = @IdPersona;",
                new { idTipoDocumento, idPersona });

            return existe == 1;
        }

        public async Task<bool> ExisteNoDocumento(string noDocumento) // Nuevo método
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"
                SELECT 1 FROM DOCUMENTO_PERSONA 
                WHERE NoDocumento = @NoDocumento;",
                new { NoDocumento = noDocumento });

            return existe == 1;
        }

        public async Task<IEnumerable<DocumentoPersona>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<DocumentoPersona>(@"
                SELECT IdTipoDocumento, IdPersona, NoDocumento, FechaCreacion, 
                       UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM DOCUMENTO_PERSONA;");
        }

        public async Task<DocumentoPersona> ObtenerPorId(int idTipoDocumento, int idPersona)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<DocumentoPersona>(@"
                SELECT IdTipoDocumento, IdPersona, NoDocumento, FechaCreacion, 
                       UsuarioCreacion, FechaModificacion, UsuarioModificacion
                FROM DOCUMENTO_PERSONA
                WHERE IdTipoDocumento = @IdTipoDocumento AND IdPersona = @IdPersona;",
                new { idTipoDocumento, idPersona });
        }

        public async Task ActualizarGeneral(DocumentoPersona documentoPersona)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                UPDATE DOCUMENTO_PERSONA
                SET NoDocumento = @NoDocumento,
                    FechaModificacion = GETDATE(), 
                    UsuarioModificacion = @UsuarioModificacion
                WHERE IdTipoDocumento = @IdTipoDocumento AND IdPersona = @IdPersona;",
                documentoPersona);
        }

        public async Task Borrar(int idTipoDocumento, int idPersona)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                DELETE FROM DOCUMENTO_PERSONA 
                WHERE IdTipoDocumento = @IdTipoDocumento AND IdPersona = @IdPersona;",
                new { idTipoDocumento, idPersona });
        }
    }
}
