using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Rol
{
    public int IdRol { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<AccionesCaosRol> AccionesCaosRols { get; set; } = new List<AccionesCaosRol>();

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
