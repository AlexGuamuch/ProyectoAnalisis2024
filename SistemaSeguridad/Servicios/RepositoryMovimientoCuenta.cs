using Dapper;
using Microsoft.Data.SqlClient;
using SistemaSeguridad.Models;

namespace SistemaSeguridad.Servicios
{
    public interface IRepositoryMovimientoCuenta
    {
        Task Crear(MovimientoCuentum movimientoCuenta);
        Task<IEnumerable<MovimientoCuentum>> Obtener();
        Task<MovimientoCuentum> ObtenerPorId(int idMovimientoCuenta);
        Task Actualizar(MovimientoCuentum movimientoCuenta);
        Task Borrar(int idMovimientoCuenta);
        Task ActualizarGeneral(MovimientoCuentum movimientoCuenta);
    }

    public class RepositoryMovimientoCuenta : IRepositoryMovimientoCuenta
    {
        private readonly string connectionString;

        public RepositoryMovimientoCuenta(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(MovimientoCuentum movimientoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var sql = @"
            INSERT INTO MOVIMIENTO_CUENTA 
            (IdSaldoCuenta, IdTipoMovimientoCXC, FechaMovimiento, ValorMovimiento, 
            ValorMovimientoPagado, GeneradoAutomaticamente, Descripcion, 
            FechaCreacion, UsuarioCreacion)
            VALUES 
            (@IdSaldoCuenta, @IdTipoMovimientoCXC, @FechaMovimiento, @ValorMovimiento, 
            @ValorMovimientoPagado, @GeneradoAutomaticamente, @Descripcion, 
            GETDATE(), @UsuarioCreacion);";

            var parameters = new
            {
                movimientoCuenta.IdSaldoCuenta,
                movimientoCuenta.IdTipoMovimientoCxc,
                movimientoCuenta.FechaMovimiento,
                movimientoCuenta.ValorMovimiento,
                movimientoCuenta.ValorMovimientoPagado,
                movimientoCuenta.GeneradoAutomaticamente,
                movimientoCuenta.Descripcion,
                movimientoCuenta.UsuarioCreacion
            };

            await connection.ExecuteAsync(sql, parameters);
        }

        public async Task<IEnumerable<MovimientoCuentum>> Obtener()
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<MovimientoCuentum>(@"
                SELECT IdMovimientoCuenta, IdSaldoCuenta, IdTipoMovimientoCXC, FechaMovimiento, 
                       ValorMovimiento, ValorMovimientoPagado, GeneradoAutomaticamente, 
                       Descripcion, FechaCreacion, UsuarioCreacion, FechaModificacion, 
                       UsuarioModificacion
                FROM MOVIMIENTO_CUENTA;");
        }

        public async Task<MovimientoCuentum> ObtenerPorId(int idMovimientoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<MovimientoCuentum>(@"
                SELECT IdMovimientoCuenta, IdSaldoCuenta, IdTipoMovimientoCXC, FechaMovimiento, 
                       ValorMovimiento, ValorMovimientoPagado, GeneradoAutomaticamente, 
                       Descripcion, FechaCreacion, UsuarioCreacion, FechaModificacion, 
                       UsuarioModificacion
                FROM MOVIMIENTO_CUENTA
                WHERE IdMovimientoCuenta = @IdMovimientoCuenta;",
                new { idMovimientoCuenta });
        }

        public async Task Actualizar(MovimientoCuentum movimientoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                UPDATE MOVIMIENTO_CUENTA
                SET IdSaldoCuenta = @IdSaldoCuenta,
                    IdTipoMovimientoCXC = @IdTipoMovimientoCXC,
                    FechaMovimiento = @FechaMovimiento,
                    ValorMovimiento = @ValorMovimiento,
                    ValorMovimientoPagado = @ValorMovimientoPagado,
                    GeneradoAutomaticamente = @GeneradoAutomaticamente,
                    Descripcion = @Descripcion,
                    FechaModificacion = GETDATE(),
                    UsuarioModificacion = @UsuarioModificacion
                WHERE IdMovimientoCuenta = @IdMovimientoCuenta;",
                movimientoCuenta);
        }

        public async Task Borrar(int idMovimientoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                DELETE FROM MOVIMIENTO_CUENTA 
                WHERE IdMovimientoCuenta = @IdMovimientoCuenta;",
                new { idMovimientoCuenta });
        }

        public Task ActualizarGeneral(MovimientoCuentum movimientoCuenta)
        {
            throw new NotImplementedException();
        }
    }
}
