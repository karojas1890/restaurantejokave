using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido1 { get; set; } = null!;

    public string? Apellido2 { get; set; }

    public string Usuario1 { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Codigo { get; set; }

    public string DocumentoIdentificacion { get; set; } = null!;

    public virtual ICollection<HistorialDeAccione> HistorialDeAcciones { get; set; } = new List<HistorialDeAccione>();

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<JornadaLaboral> JornadaLaborals { get; set; } = new List<JornadaLaboral>();

    public virtual ICollection<Mesa> Mesas { get; set; } = new List<Mesa>();

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();

    public virtual ICollection<Tarjetum> Tarjeta { get; set; } = new List<Tarjetum>();

    public virtual ICollection<Visitum> Visita { get; set; } = new List<Visitum>();
}
