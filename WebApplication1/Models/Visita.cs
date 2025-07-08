using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Visitum
{
    public int IdVisita { get; set; }

    public int IdUsuario { get; set; }

    public DateTime FechaHoraIngreso { get; set; }

    public DateTime? FechaHoraSalida { get; set; }

    public int? IdSilla { get; set; }

    public int Estado { get; set; }

    public int? EstadoAnimo { get; set; }

    public int? Satisfaccion { get; set; }

    public string? Comentarios { get; set; }

    public virtual Silla? IdSillaNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
}
