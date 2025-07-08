using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Operacion
{
    public int IdOperacion { get; set; }

    public int Estado { get; set; }

    public DateTime FechaHoraInicio { get; set; }

    public DateTime? FechaHoraFin { get; set; }

    public int TiempoMaximo { get; set; }

    public virtual ICollection<HistorialDeCaos> HistorialDeCaos { get; set; } = new List<HistorialDeCaos>();
}
