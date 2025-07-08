using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Activo
{
    public int IdActivo { get; set; }

    public string Descripcion { get; set; } = null!;

    public int Categoria { get; set; }

    public int Estado { get; set; }

    public virtual ICollection<TiposDeCaos> TiposDeCaos { get; set; } = new List<TiposDeCaos>();
}
