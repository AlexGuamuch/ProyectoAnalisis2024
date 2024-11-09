using System;
using System.Collections.Generic;

namespace SistemaSeguridad.Models;

public partial class TipoMovimientoCxc
{
    public int IdTipoMovimientoCxc { get; set; }

    public string Nombre { get; set; }

    public int OperacionCuentaCorriente { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string UsuarioModificacion { get; set; }
}
