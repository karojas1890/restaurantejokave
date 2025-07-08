using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class SeccionesRestaurante
{
    public int IdSeccion { get; set; }

    public int Estado { get; set; }

    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();

    public virtual ICollection<TiposDeCaos> TiposDeCaos { get; set; } = new List<TiposDeCaos>();
}
