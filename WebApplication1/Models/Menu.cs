using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Menu
{
    public int IdProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public int Categoria { get; set; }

    public double Precio { get; set; }

    public string? DireccionImagen { get; set; }

    public string? Descripcion { get; set; }

    public bool? EsActivo { get; set; }

    public virtual ICollection<DetalleOrden> DetalleOrdens { get; set; } = new List<DetalleOrden>();
}
