using System;
using System.Collections.Generic;

namespace SistemaSeguridad.Models;

public partial class DocumentoPersona
{
    public int IdTipoDocumento { get; set; }

    public int IdPersona { get; set; }

    public string NoDocumento { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public string UsuarioModificacion { get; set; }
}
