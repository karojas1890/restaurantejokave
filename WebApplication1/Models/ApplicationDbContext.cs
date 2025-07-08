using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccionesCaosRol> AccionesCaosRols { get; set; }

    public virtual DbSet<Activo> Activos { get; set; }

    public virtual DbSet<Cuentum> Cuenta { get; set; }

    public virtual DbSet<DetalleOrden> DetalleOrdens { get; set; }

    public virtual DbSet<HistorialDeAccione> HistorialDeAcciones { get; set; }

    public virtual DbSet<HistorialDeCaos> HistorialDeCaos { get; set; }

    public virtual DbSet<Insumo> Insumos { get; set; }

    public virtual DbSet<JornadaLaboral> JornadaLaborals { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Mesa> Mesas { get; set; }

    public virtual DbSet<Operacion> Operacions { get; set; }

    public virtual DbSet<Orden> Ordens { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<SeccionesRestaurante> SeccionesRestaurantes { get; set; }

    public virtual DbSet<Silla> Sillas { get; set; }

    public virtual DbSet<Tarjetum> Tarjeta { get; set; }

    public virtual DbSet<TipoDeAccion> TipoDeAccions { get; set; }

    public virtual DbSet<TiposDeCaos> TiposDeCaos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Visita> Visita { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tiusr24pl.cuc-carrera-ti.ac.cr;Database=tiusr24pl_jokave;User id=usuariojokave;Password=jokave2025;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("usuariojokave");

        modelBuilder.Entity<AccionesCaosRol>(entity =>
        {
            entity.HasKey(e => new { e.IdCaos, e.IdAccion, e.IdRolAutorizado });

            entity.ToTable("AccionesCaosRol");

            entity.HasOne(d => d.IdAccionNavigation).WithMany(p => p.AccionesCaosRols)
                .HasForeignKey(d => d.IdAccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccionesCaosRol_Accion");

            entity.HasOne(d => d.IdCaosNavigation).WithMany(p => p.AccionesCaosRols)
                .HasForeignKey(d => d.IdCaos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccionesCaosRol_Caos");

            entity.HasOne(d => d.IdRolAutorizadoNavigation).WithMany(p => p.AccionesCaosRols)
                .HasForeignKey(d => d.IdRolAutorizado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AccionesCaosRol_Rol");
        });

        modelBuilder.Entity<Activo>(entity =>
        {
            entity.HasKey(e => e.IdActivo).HasName("PK__Activos__146481C08388A3EC");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(1);
        });

        modelBuilder.Entity<Cuentum>(entity =>
        {
            entity.HasKey(e => e.IdCuenta).HasName("PK__Cuenta__D41FD706D2D05A4F");

            entity.HasIndex(e => e.IdOrden, "UQ__Cuenta__C38F300C0EACE7B3").IsUnique();

            entity.Property(e => e.Estado).HasDefaultValue(2);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(12, 2)")
                .HasColumnName("IVA");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.Total).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdOrdenNavigation).WithOne(p => p.Cuentum)
                .HasForeignKey<Cuentum>(d => d.IdOrden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cuenta_Orden");
        });

        modelBuilder.Entity<DetalleOrden>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__DetalleO__E43646A5AFB30DC0");

            entity.ToTable("DetalleOrden");

            entity.Property(e => e.SubTotal)
                .HasComputedColumnSql("([CantidadProducto]*[ValorIndividual])", true)
                .HasColumnType("decimal(21, 2)");
            entity.Property(e => e.ValorIndividual).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdOrdenNavigation).WithMany(p => p.DetalleOrdens)
                .HasForeignKey(d => d.IdOrden)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleOrden_Orden");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleOrdens)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetalleOrden_Menu");
        });

        modelBuilder.Entity<HistorialDeAccione>(entity =>
        {
            entity.HasKey(e => e.IdRegistro).HasName("PK__Historia__FFA45A99AB9F6042");

            entity.Property(e => e.HoraSuceso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdAccionNavigation).WithMany(p => p.HistorialDeAcciones)
                .HasForeignKey(d => d.IdAccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistorialAcciones_Accion");

            entity.HasOne(d => d.IdIncidenteNavigation).WithMany(p => p.HistorialDeAcciones)
                .HasForeignKey(d => d.IdIncidente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistorialAcciones_Incidente");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialDeAcciones)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistorialAcciones_Usuario");
        });

        modelBuilder.Entity<HistorialDeCaos>(entity =>
        {
            entity.HasKey(e => e.IdIncidente).HasName("PK__Historia__E92B13DF53A81E80");

            entity.Property(e => e.HoraSuceso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdCaosNavigation).WithMany(p => p.HistorialDeCaos)
                .HasForeignKey(d => d.IdCaos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistorialCaos_TipoCaos");

            entity.HasOne(d => d.IdOperacionNavigation).WithMany(p => p.HistorialDeCaos)
                .HasForeignKey(d => d.IdOperacion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HistorialCaos_Operacion");
        });

        modelBuilder.Entity<Insumo>(entity =>
        {
            entity.HasKey(e => e.IdInsumo).HasName("PK__Insumo__F378A2AF7BB89E54");

            entity.ToTable("Insumo");

            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<JornadaLaboral>(entity =>
        {
            entity.HasKey(e => new { e.IdJornadaLaboral, e.IdUsuario });

            entity.ToTable("JornadaLaboral");

            entity.Property(e => e.IdJornadaLaboral).ValueGeneratedOnAdd();
            entity.Property(e => e.FechaHoraInicioSesion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.JornadaLaborals)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JornadaLaboral_Usuario");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Menu__09889210F76A2449");

            entity.ToTable("Menu");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DireccionImagen)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EsActivo).HasDefaultValue(true);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Mesa>(entity =>
        {
            entity.HasKey(e => e.IdMesa).HasName("PK__Mesas__4D7E81B16EEF6336");

            entity.Property(e => e.Estado).HasDefaultValue(1);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaUltimaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdSeccionNavigation).WithMany(p => p.Mesas)
                .HasForeignKey(d => d.IdSeccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mesas_Secciones");

            entity.HasOne(d => d.MeseroAsignadoNavigation).WithMany(p => p.Mesas)
                .HasForeignKey(d => d.MeseroAsignado)
                .HasConstraintName("FK_Mesas_Usuarios");
        });

        modelBuilder.Entity<Operacion>(entity =>
        {
            entity.HasKey(e => e.IdOperacion).HasName("PK__Operacio__7778456BAE2CEF12");

            entity.ToTable("Operacion");

            entity.Property(e => e.Estado).HasDefaultValue(1);
            entity.Property(e => e.FechaHoraFin).HasColumnType("datetime");
            entity.Property(e => e.FechaHoraInicio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Orden>(entity =>
        {
            entity.HasKey(e => e.IdOrden).HasName("PK__Orden__C38F300DAC5A8696");

            entity.ToTable("Orden");

            entity.Property(e => e.HoraRecibida)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Total)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdMesaNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.IdMesa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_Mesa");

            entity.HasOne(d => d.MeseroAsignadoNavigation).WithMany(p => p.Ordens)
                .HasForeignKey(d => d.MeseroAsignado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_Mesero");

            entity.HasOne(d => d.Visitum).WithMany(p => p.Ordens)
                .HasForeignKey(d => new { d.IdVisita, d.IdUsuario })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orden_Visita");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584C8685A9EF");

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SeccionesRestaurante>(entity =>
        {
            entity.HasKey(e => e.IdSeccion).HasName("PK__Seccione__CD2B049F1DD4967B");

            entity.ToTable("SeccionesRestaurante");

            entity.Property(e => e.Estado).HasDefaultValue(1);
        });

        modelBuilder.Entity<Silla>(entity =>
        {
            entity.HasKey(e => e.IdSilla).HasName("PK__Sillas__510682B7D943C464");

            entity.Property(e => e.Estado).HasDefaultValue(1);

            entity.HasOne(d => d.IdMesaNavigation).WithMany(p => p.Sillas)
                .HasForeignKey(d => d.IdMesa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sillas_Mesas");
        });

        modelBuilder.Entity<Tarjetum>(entity =>
        {
            entity.HasKey(e => e.NumeroTarjeta).HasName("PK__Tarjeta__BC163C0BFCFF73DD");

            entity.Property(e => e.NumeroTarjeta)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.CodigoSeguridad)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.EsActiva).HasDefaultValue(true);
            entity.Property(e => e.Saldo)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Tarjeta)
                .HasForeignKey(d => d.IdCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarjeta_Cliente");
        });

        modelBuilder.Entity<TipoDeAccion>(entity =>
        {
            entity.HasKey(e => e.IdTipoAccion).HasName("PK__TipoDeAc__F9AE1650F0AB8F04");

            entity.ToTable("TipoDeAccion");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TiposDeCaos>(entity =>
        {
            entity.HasKey(e => e.IdCaos).HasName("PK__TiposDeC__3B7B0B5D5CFC7997");

            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PerdidaEconomicaEstimada).HasColumnType("decimal(12, 2)");

            entity.HasOne(d => d.CategoriaEquipoDaniaNavigation).WithMany(p => p.TiposDeCaos)
                .HasForeignKey(d => d.CategoriaEquipoDania)
                .HasConstraintName("FK_Caos_CategoriaActivos");

            entity.HasOne(d => d.CategoriaInsumosDesabasteceNavigation).WithMany(p => p.TiposDeCaos)
                .HasForeignKey(d => d.CategoriaInsumosDesabastece)
                .HasConstraintName("FK_Caos_CategoriaInsumos");

            entity.HasOne(d => d.IdSeccionNavigation).WithMany(p => p.TiposDeCaos)
                .HasForeignKey(d => d.IdSeccion)
                .HasConstraintName("FK_TiposDeCaos_Seccion");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF97DE8400C8");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.DocumentoIdentificacion, "UQ__Usuario__46519A5BDDF3D296").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Usuario__A9D105345125277B").IsUnique();

            entity.HasIndex(e => e.Usuario1, "UQ__Usuario__E3237CF7892D08E2").IsUnique();

            entity.Property(e => e.Apellido1)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Apellido2)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Codigo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Contraseña)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DocumentoIdentificacion)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Usuario1)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("Usuario");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuario_Rol");
        });

        modelBuilder.Entity<Visita>(entity =>
        {
            entity.HasKey(e => new { e.IdVisita, e.IdUsuario });

            entity.Property(e => e.IdVisita).ValueGeneratedOnAdd();
            entity.Property(e => e.Comentarios)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Estado).HasDefaultValue(1);
            entity.Property(e => e.FechaHoraIngreso)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaHoraSalida).HasColumnType("datetime");

            entity.HasOne(d => d.IdSillaNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.IdSilla)
                .HasConstraintName("FK_Visita_Silla");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Visita)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Visita_Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
