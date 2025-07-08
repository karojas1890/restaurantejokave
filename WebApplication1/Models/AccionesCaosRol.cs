using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class AccionesCaosRol
{
    public int IdCaos { get; set; }

    public int IdAccion { get; set; }

    public int IdRolAutorizado { get; set; }

    public virtual TipoDeAccion IdAccionNavigation { get; set; } = null!;

    public virtual TiposDeCao IdCaosNavigation { get; set; } = null!;

    public virtual Rol IdRolAutorizadoNavigation { get; set; } = null!;
}
