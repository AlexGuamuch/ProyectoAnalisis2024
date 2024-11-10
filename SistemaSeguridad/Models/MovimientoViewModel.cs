namespace SistemaSeguridad.Models
{
    public class MovimientoViewModel
    {
        public SaldoCuentum SaldoCuentum { get; set; }
        public IEnumerable<MovimientoCuentum> MovimientoCuenta { get; set; }
    }
}
