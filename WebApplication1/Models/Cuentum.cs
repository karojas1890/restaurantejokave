using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Cuentum
{
    public int IdCuenta { get; set; }

    public int IdOrden { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }

    public int Estado { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual Orden IdOrdenNavigation { get; set; } = null!;
}
