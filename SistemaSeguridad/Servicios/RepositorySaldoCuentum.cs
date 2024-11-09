using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration; // Asegúrate de que este using esté presente
using SistemaSeguridad.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositorySaldoCuenta
    {
        Task<IEnumerable<SaldoCuentum>> BuscarPorNombre(string nombre);
        Task<IEnumerable<SaldoCuentum>> BuscarPorDpi(string noDocumento);
        Task<IEnumerable<SaldoCuentum>> BuscarPorNumeroCuenta(string numeroCuenta);
    }

    public class RepositorySaldoCuenta : IRepositorySaldoCuenta
    {
        private readonly string connectionString;

        public RepositorySaldoCuenta(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Búsqueda por nombre
        public async Task<IEnumerable<SaldoCuentum>> BuscarPorNombre(string nombre)
        {
            using var connection = new SqlConnection(connectionString);
            var query = @"
                SELECT SC.*  -- Seleccionamos las columnas de Saldo_Cuenta
                FROM SALDO_CUENTA SC
                INNER JOIN PERSONA P ON SC.IdPersona = P.IdPersona
                WHERE P.Nombre LIKE '%' + @Nombre + '%';"; // Asegúrate de que el parámetro esté bien definido

            return await connection.QueryAsync<SaldoCuentum>(query, new { Nombre = nombre });
        }

        // Búsqueda por DPI (NoDocumento)
        public async Task<IEnumerable<SaldoCuentum>> BuscarPorDpi(string noDocumento)
        {
            using var connection = new SqlConnection(connectionString);
            var query = @"
               SELECT SC.*, P.*, DP.NoDocumento
        FROM SALDO_CUENTA SC
        INNER JOIN PERSONA P ON SC.IdPersona = P.IdPersona
        INNER JOIN DOCUMENTO_PERSONA DP ON P.IdPersona = DP.IdPersona
        WHERE DP.NoDocumento = @NoDocumento";  // Cambié Dpi a NoDocumento

            return await connection.QueryAsync<SaldoCuentum>(query, new { NoDocumento = noDocumento });
        }

        // Búsqueda por número de cuenta
        public async Task<IEnumerable<SaldoCuentum>> BuscarPorNumeroCuenta(string numeroCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var query = @"
                SELECT * 
                FROM SALDO_CUENTA 
                WHERE IdSaldoCuenta = @NumeroCuenta;"; // Aquí se usa el mismo nombre de parámetro

            return await connection.QueryAsync<SaldoCuentum>(query, new { NumeroCuenta = numeroCuenta });
        }
    }
}
