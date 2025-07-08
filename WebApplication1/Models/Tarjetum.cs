using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Tarjetum
{
    public string NumeroTarjeta { get; set; } = null!;

    public int IdCliente { get; set; }

    public int MesVencimiento { get; set; }

    public int AnioVencimiento { get; set; }

    public string CodigoSeguridad { get; set; } = null!;

    public decimal? Saldo { get; set; }

    public bool? EsActiva { get; set; }

    public virtual Usuario IdClienteNavigation { get; set; } = null!;
}
