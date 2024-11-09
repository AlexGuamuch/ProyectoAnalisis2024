using System;
using System.Collections.Generic;

namespace SistemaSeguridad.Models;

public partial class TipoDocumento
{
    public int IdTipoDocumento { get; set; }

    public string Nombre { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string UsuarioModificacion { get; set; }
}
