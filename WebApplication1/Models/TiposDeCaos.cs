using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class TiposDeCaos
{
    public int IdCaos { get; set; }

    public string Nombre { get; set; } = null!;

    public int TiempoTotalDestruccion { get; set; }

    public decimal PerdidaEconomicaEstimada { get; set; }

    public int? CategoriaEquipoDania { get; set; }

    public int? CategoriaInsumosDesabastece { get; set; }

    public int? IdSeccion { get; set; }

    public virtual ICollection<AccionesCaosRol> AccionesCaosRols { get; set; } = new List<AccionesCaosRol>();

    public virtual Activo? CategoriaEquipoDaniaNavigation { get; set; }

    public virtual Insumo? CategoriaInsumosDesabasteceNavigation { get; set; }

    public virtual ICollection<HistorialDeCaos> HistorialDeCaos { get; set; } = new List<HistorialDeCaos>();

    public virtual SeccionesRestaurante? IdSeccionNavigation { get; set; }
}
