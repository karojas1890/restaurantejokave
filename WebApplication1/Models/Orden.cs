using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Orden
{
    public int IdOrden { get; set; }

    public int IdVisita { get; set; }

    public int IdUsuario { get; set; }

    public int IdMesa { get; set; }

    public int MeseroAsignado { get; set; }

    public int EstadoOrden { get; set; }

    public decimal? Total { get; set; }

    public DateTime? HoraRecibida { get; set; }

    public virtual Cuentum? Cuentum { get; set; }

    public virtual ICollection<DetalleOrden> DetalleOrdens { get; set; } = new List<DetalleOrden>();

    public virtual Mesa IdMesaNavigation { get; set; } = null!;

    public virtual Usuario MeseroAsignadoNavigation { get; set; } = null!;

    public virtual Visita Visitum { get; set; } = null!;
}
