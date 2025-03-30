using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.Model.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace SistemaVenta.DAL.DBContext;

public partial class DbventaContext : DbContext
{
    public DbventaContext()
    {
    }

    public DbventaContext(DbContextOptions<DbventaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; } // Cambiado a plural para convención
    public virtual DbSet<DetalleVenta> DetalleVentas { get; set; } // Cambiado a plural
    public virtual DbSet<Menu> Menus { get; set; }
    public virtual DbSet<MenuRol> MenuRols { get; set; }
    public virtual DbSet<NumeroDocumento> NumeroDocumentos { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<Rol> Rols { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Venta> Ventas { get; set; } // Cambiado a plural

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp"); // Para soporte de UUID si es necesario

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("pk_categoria");
            entity.ToTable("categoria"); // Nombre de tabla en minúsculas para PostgreSQL

            entity.Property(e => e.IdCategoria).HasColumnName("idcategoria");
            entity.Property(e => e.EsActivo)
                .HasDefaultValue(true)
                .HasColumnName("esactivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("NOW()") // Cambiado a NOW() para PostgreSQL
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<DetalleVenta>(entity =>
        {
            entity.HasKey(e => e.IdDetalleVenta).HasName("pk_detalleventa");
            entity.ToTable("detalleventa"); // Nombre de tabla en minúsculas

            entity.Property(e => e.IdDetalleVenta).HasColumnName("iddetalleventa");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdProducto).HasColumnName("idproducto");
            entity.Property(e => e.IdVenta).HasColumnName("idventa");
            entity.Property(e => e.Precio)
                .HasColumnType("numeric(10,2)")
                .HasColumnName("precio");
            entity.Property(e => e.Total)
                .HasColumnType("numeric(10,2)")
                .HasColumnName("total");

            entity.HasOne(d => d.IdProductoNavigation)
                .WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("fk_detalleventa_producto");

            entity.HasOne(d => d.IdVentaNavigation)
                .WithMany(p => p.DetalleVenta)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("fk_detalleventa_venta");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu).HasName("pk_menu");
            entity.ToTable("menu");

            entity.Property(e => e.IdMenu).HasColumnName("idmenu");
            entity.Property(e => e.Icono)
                .HasMaxLength(50)
                .HasColumnName("icono");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.Url)
                .HasMaxLength(50)
                .HasColumnName("url");
        });

        modelBuilder.Entity<MenuRol>(entity =>
        {
            entity.HasKey(e => e.IdMenuRol).HasName("pk_menurol");
            entity.ToTable("menurol");

            entity.Property(e => e.IdMenuRol).HasColumnName("idmenurol");
            entity.Property(e => e.IdMenu).HasColumnName("idmenu");
            entity.Property(e => e.IdRol).HasColumnName("idrol");

            entity.HasOne(d => d.IdMenuNavigation)
                .WithMany(p => p.MenuRols)
                .HasForeignKey(d => d.IdMenu)
                .HasConstraintName("fk_menurol_menu");

            entity.HasOne(d => d.IdRolNavigation)
                .WithMany(p => p.MenuRols)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("fk_menurol_rol");
        });

        modelBuilder.Entity<NumeroDocumento>(entity =>
        {
            entity.HasKey(e => e.IdNumeroDocumento).HasName("pk_numerodocumento");
            entity.ToTable("numerodocumento");

            entity.Property(e => e.IdNumeroDocumento).HasColumnName("idnumerodocumento");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("NOW()")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.UltimoNumero).HasColumnName("ultimo_numero");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("pk_producto");
            entity.ToTable("producto");

            entity.Property(e => e.IdProducto).HasColumnName("idproducto");
            entity.Property(e => e.EsActivo)
                .HasDefaultValue(true)
                .HasColumnName("esactivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("NOW()")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.IdCategoria).HasColumnName("idcategoria");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasColumnType("numeric(10,2)")
                .HasColumnName("precio");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.Foto)
                   .HasMaxLength(100)
                   .HasColumnName("foto");

            entity.HasOne(d => d.IdCategoriaNavigation)
                .WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("fk_producto_categoria");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("pk_rol");
            entity.ToTable("rol");

            entity.Property(e => e.IdRol).HasColumnName("idrol");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("NOW()")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("pk_usuario");
            entity.ToTable("usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("idusuario");
            entity.Property(e => e.Clave)
                .HasMaxLength(40)
                .HasColumnName("clave");
            entity.Property(e => e.Correo)
                .HasMaxLength(40)
                .HasColumnName("correo");
            entity.Property(e => e.EsActivo)
                .HasDefaultValue(true)
                .HasColumnName("esactivo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("NOW()")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.IdRol).HasColumnName("idrol");
            entity.Property(e => e.NombreCompleto)
                .HasMaxLength(100)
                .HasColumnName("nombrecompleto");

            entity.HasOne(d => d.IdRolNavigation)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("fk_usuario_rol");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("pk_venta");
            entity.ToTable("venta");

            entity.Property(e => e.IdVenta).HasColumnName("idventa");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("NOW()")
                .HasColumnName("fecharegistro");
            entity.Property(e => e.NumeroDocumento)
                .HasMaxLength(40)
                .HasColumnName("numerodocumento");
            entity.Property(e => e.TipoPago)
                .HasMaxLength(50)
                .HasColumnName("tipopago");
            entity.Property(e => e.Total)
                .HasColumnType("numeric(10,2)")
                .HasColumnName("total");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}