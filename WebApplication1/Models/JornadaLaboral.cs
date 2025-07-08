using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class JornadaLaboral
{
    public int IdJornadaLaboral { get; set; }

    public int IdUsuario { get; set; }

    public DateTime FechaHoraInicioSesion { get; set; }

    public int Estado { get; set; }

    public int? IndiceEnergia { get; set; }

    public int? IndiceEstres { get; set; }

    public int? IndiceEficiencia { get; set; }

    public int? RiesgoAbandono { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
