using System;
using System.Collections.Generic;

namespace SistemaSeguridad.Models;

public partial class SaldoCuentum
{
    public int IdSaldoCuenta { get; set; }

    public int IdPersona { get; set; }

    public int IdStatusCuenta { get; set; }

    public int IdTipoSaldoCuenta { get; set; }

    public decimal? SaldoAnterior { get; set; }

    public decimal? Debitos { get; set; }

    public decimal? Creditos { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string UsuarioModificacion { get; set; }

    public virtual Persona IdPersonaNavigation { get; set; }

    public virtual StatusCuentum IdStatusCuentaNavigation { get; set; }

    public virtual TipoSaldoCuentum IdTipoSaldoCuentaNavigation { get; set; }

    public virtual ICollection<MovimientoCuentum> MovimientoCuenta { get; set; } = new List<MovimientoCuentum>();

    public string StatusCuentaNombre { get; set; }

    public string TipoSaldoCuentaNombre { get; set; }
}
