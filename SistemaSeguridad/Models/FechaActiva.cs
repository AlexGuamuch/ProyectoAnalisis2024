using System;
using System.Collections.Generic;

namespace SistemaSeguridad.Models;

public partial class FechaActiva
{
    public DateOnly FechaInicial { get; set; }

    public DateOnly FechaFinal { get; set; }
}
