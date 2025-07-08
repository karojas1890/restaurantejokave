using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class DetalleOrden
{
    public int IdDetalle { get; set; }

    public int IdOrden { get; set; }

    public int IdProducto { get; set; }

    public int CantidadProducto { get; set; }

    public decimal ValorIndividual { get; set; }

    public decimal? SubTotal { get; set; }

    public virtual Orden IdOrdenNavigation { get; set; } = null!;

    public virtual Menu IdProductoNavigation { get; set; } = null!;
}
