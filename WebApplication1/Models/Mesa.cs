using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Mesa
{
    public int IdMesa { get; set; }

    public int IdSeccion { get; set; }

    public int Capacidad { get; set; }

    public int Estado { get; set; }

    public int? MeseroAsignado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaUltimaActualizacion { get; set; }

    public virtual SeccionesRestaurante IdSeccionNavigation { get; set; } = null!;

    public virtual Usuario? MeseroAsignadoNavigation { get; set; }

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    public virtual ICollection<Silla> Sillas { get; set; } = new List<Silla>();
}
