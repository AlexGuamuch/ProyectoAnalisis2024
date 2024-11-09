using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaSeguridad.Models;

public partial class Persona
{
    public int IdPersona { get; set; }

    public string Nombre { get; set; }

    public string Apellido { get; set; }

    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; } = DateTime.Today;

    public int IdGenero { get; set; }

    public string Direccion { get; set; }

    public string Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El campo debe ser un correo valido")]
    public string CorreoElectronico { get; set; }

    public int IdEstadoCivil { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string UsuarioCreacion { get; set; }

    public DateTime FechaModificacion { get; set; }

    public string UsuarioModificacion { get; set; }

    public virtual EstadoCivil IdEstadoCivilNavigation { get; set; }

    public virtual ICollection<SaldoCuentum> SaldoCuenta { get; set; } = new List<SaldoCuentum>();
}
