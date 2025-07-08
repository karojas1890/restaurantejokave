using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class HistorialDeAccione
{
    public int IdRegistro { get; set; }

    public int IdIncidente { get; set; }

    public int IdAccion { get; set; }

    public int IdUsuario { get; set; }

    public DateTime HoraSuceso { get; set; }

    public int DuracionSegundos { get; set; }

    public int Efectividad { get; set; }

    public virtual TipoDeAccion IdAccionNavigation { get; set; } = null!;

    public virtual HistorialDeCaos IdIncidenteNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
