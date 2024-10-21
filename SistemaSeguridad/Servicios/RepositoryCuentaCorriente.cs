using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryCuentaCorriente
    {
        Task<IEnumerable<SaldoCuentum>> ObtenerCuentas();
        Task<IEnumerable<SaldoCuentum>> BuscarCuentaPorPersona(string personaId);
        Task RegistrarMovimiento(MovimientoCuentum movimiento);
        Task<MovimientoCuentum> ObtenerMovimientoPorId(int idMovimiento);
        Task EliminarMovimiento(int idMovimiento);
    }

    public class RepositoryCuentaCorriente : IRepositoryCuentaCorriente
    {
        private readonly string connectionString;

        public RepositoryCuentaCorriente(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Obtener todas las cuentas
        public async Task<IEnumerable<SaldoCuentum>> ObtenerCuentas()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<SaldoCuentum>("SELECT * FROM SALDO_CUENTA");
        }

        // Buscar cuenta por persona (cliente)
        public async Task<IEnumerable<SaldoCuentum>> BuscarCuentaPorPersona(string personaId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<SaldoCuentum>(
                "SELECT * FROM SALDO_CUENTA WHERE IdPersona = @PersonaId",
                new { PersonaId = personaId });
        }

        // Registrar un movimiento en una cuenta
        public async Task RegistrarMovimiento(MovimientoCuentum movimiento)
        {
            using var connection = new SqlConnection(connectionString);

            // Insertar el movimiento (código existente)
            await connection.ExecuteAsync(
                @"INSERT INTO MOVIMIENTO_CUENTA (
            IdSaldoCuenta, IdTipoMovimientoCXC, FechaMovimiento, 
            ValorMovimiento, ValorMovimientoPagado, GeneradoAutomaticamente, 
            Descripcion, FechaCreacion, UsuarioCreacion)
                VALUES (
            @IdSaldoCuenta, @IdTipoMovimientoCXC, @FechaMovimiento, 
            @ValorMovimiento, @ValorMovimientoPagado, @GeneradoAutomaticamente, 
            @Descripcion, @FechaCreacion, @UsuarioCreacion)",
                movimiento);

            // Actualizar el saldo de la cuenta
            var nuevoSaldo = await ActualizarSaldoCuenta(movimiento);
        }

        public async Task<decimal> ActualizarSaldoCuenta(MovimientoCuentum movimiento)
        {
            using var connection = new SqlConnection(connectionString);
            var nuevoSaldo = await connection.QuerySingleAsync<decimal>("spActualizarSaldoCuenta",
                new
                {
                    IdSaldoCuenta = movimiento.IdSaldoCuenta,
                    ValorMovimiento = movimiento.ValorMovimiento,
                    EsAbono = movimiento.IdTipoMovimientoCxc == 1
                },
                commandType: System.Data.CommandType.StoredProcedure);

            return nuevoSaldo;
        }

        // Obtener un movimiento por su ID
        public async Task<MovimientoCuentum> ObtenerMovimientoPorId(int idMovimiento)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<MovimientoCuentum>(
                "SELECT * FROM MOVIMIENTO_CUENTA WHERE IdMovimientoCuenta = @IdMovimiento",
                new { IdMovimiento = idMovimiento });
        }

        // Eliminar un movimiento
        public async Task EliminarMovimiento(int idMovimiento)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                "DELETE FROM MOVIMIENTO_CUENTA WHERE IdMovimientoCuenta = @IdMovimiento",
                new { IdMovimiento = idMovimiento });
        }
    }
}
