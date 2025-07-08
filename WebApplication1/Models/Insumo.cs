using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Insumo
{
    public int IdInsumo { get; set; }

    public string Nombre { get; set; } = null!;

    public int Categoria { get; set; }

    public string UnidadMedida { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<TiposDeCaos> TiposDeCaos { get; set; } = new List<TiposDeCaos>();
}
