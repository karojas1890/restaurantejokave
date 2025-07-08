using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class TipoDeAccion
{
    public int IdTipoAccion { get; set; }

    public string Descripcion { get; set; } = null!;

    public int? Impacto { get; set; }

    public virtual ICollection<AccionesCaosRol> AccionesCaosRols { get; set; } = new List<AccionesCaosRol>();

    public virtual ICollection<HistorialDeAccione> HistorialDeAcciones { get; set; } = new List<HistorialDeAccione>();
}
