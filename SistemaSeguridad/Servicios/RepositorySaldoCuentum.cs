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
        Task<IEnumerable<SaldoCuentum>> ObtenerCuentasId(int idPersona);
        Task<SaldoCuentum> ObtenerPorId(int idSaldoCuenta);
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
                WHERE P.Nombre LIKE '%' + @Nombre + '%';";

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
                WHERE DP.NoDocumento = @NoDocumento";

            return await connection.QueryAsync<SaldoCuentum>(query, new { NoDocumento = noDocumento });
        }

        // Búsqueda por número de cuenta
        public async Task<IEnumerable<SaldoCuentum>> BuscarPorNumeroCuenta(string numeroCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var query = @"
                SELECT * 
                FROM SALDO_CUENTA 
                WHERE IdSaldoCuenta = @NumeroCuenta;";

            return await connection.QueryAsync<SaldoCuentum>(query, new { NumeroCuenta = numeroCuenta });
        }

        public async Task<IEnumerable<SaldoCuentum>> ObtenerCuentasId(int idPersona)
        {
            using var connection = new SqlConnection(connectionString);
            var query = @"select sc.IdSaldoCuenta, st.Nombre as StatusCuentaNombre, tsc.Nombre as TipoSaldoCuentaNombre, sc.SaldoAnterior, sc.Debitos, sc.Creditos from SALDO_CUENTA sc
                         inner join STATUS_CUENTA st on sc.IdStatusCuenta = st.IdStatusCuenta
                         inner join TIPO_SALDO_CUENTA tsc on sc.IdTipoSaldoCuenta = tsc.IdTipoSaldoCuenta
                         where IdPersona = @IdPersona";
            return await connection.QueryAsync<SaldoCuentum>(query, new { IdPersona = idPersona });
        }

        public async Task<SaldoCuentum> ObtenerPorId(int idSaldoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<SaldoCuentum>(@"
                select IdSaldoCuenta,IdPersona,IdStatusCuenta,IdSaldoCuenta,SaldoAnterior,Debitos,Creditos from SALDO_CUENTA
                WHERE IdSaldoCuenta = @IdSaldoCuenta;",
                new { idSaldoCuenta = idSaldoCuenta });
        }
    }
}
