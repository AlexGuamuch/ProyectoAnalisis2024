using System;
using System.Collections.Generic;

namespace SistemaSeguridad.Models;

public partial class MovimientoCuentum
{
    public int IdMovimientoCuenta { get; set; }

    public int IdSaldoCuenta { get; set; }

    public int IdTipoMovimientoCxc { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public decimal ValorMovimiento { get; set; }

    public decimal ValorMovimientoPagado { get; set; }

    public bool GeneradoAutomaticamente { get; set; }

    public string Descripcion { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string UsuarioModificacion { get; set; }

    public virtual SaldoCuentum IdSaldoCuentaNavigation { get; set; }

    public virtual TipoMovimientoCxc IdTipoMovimientoCxcNavigation { get; set; }
}
