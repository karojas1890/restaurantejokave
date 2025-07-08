using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Silla
{
    public int IdSilla { get; set; }

    public int IdMesa { get; set; }

    public int Estado { get; set; }

    public virtual Mesa IdMesaNavigation { get; set; } = null!;

    public virtual ICollection<Visita> Visita { get; set; } = new List<Visita>();
}
