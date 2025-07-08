using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class HistorialDeCao
{
    public int IdIncidente { get; set; }

    public int IdCaos { get; set; }

    public DateTime HoraSuceso { get; set; }

    public bool Resultado { get; set; }

    public int IdOperacion { get; set; }

    public virtual ICollection<HistorialDeAccione> HistorialDeAcciones { get; set; } = new List<HistorialDeAccione>();

    public virtual TiposDeCao IdCaosNavigation { get; set; } = null!;

    public virtual Operacion IdOperacionNavigation { get; set; } = null!;
}
