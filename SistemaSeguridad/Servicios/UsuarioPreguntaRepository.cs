using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;
using Dapper;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryUsuarioPregunta
    {
        Task Crear(UsuarioPregunta pregunta);
        Task<List<UsuarioPregunta>> ObtenerPorUsuario(string idUsuario);
    }
    public class UsuarioPreguntaRepository : IRepositoryUsuarioPregunta
    {
        private readonly string connectionString;
        public UsuarioPreguntaRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public UsuarioPreguntaRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task Crear(UsuarioPregunta pregunta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("spusuario_pregunta_inserta",
                                                             new
                                                             {
                                                                 IdUsuario = pregunta.IdUsuario,
                                                                 Pregunta = pregunta.Pregunta,
                                                                 Respuesta = pregunta.Respuesta,
                                                                 OrdenPregunta = pregunta.OrdenPregunta,
                                                                 FechaCreacion = pregunta.FechaCreacion,
                                                                 UsuarioCreacion = pregunta.UsuarioCreacion
                                                             },
                                                             commandType: System.Data.CommandType.StoredProcedure);
            pregunta.IdPregunta = id;
        }

        public async Task<List<UsuarioPregunta>> ObtenerPorUsuario(string idUsuario)
        {
            using var connection = new SqlConnection(connectionString);
            var preguntas = await connection.QueryAsync<UsuarioPregunta>("spusuario_pregunta_obtener_por_usuario",
                                                                        new { IdUsuario = idUsuario },
                                                                        commandType: System.Data.CommandType.StoredProcedure);
            return preguntas.ToList();
        }
    }
}