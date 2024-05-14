using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Inventario.Models;

public partial class Inventario1Context : DbContext
{
    public Inventario1Context()
    {
    }

    public Inventario1Context(DbContextOptions<Inventario1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<MovimientoProducto> MovimientosProductos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.Status).HasName("PK__Estados__3A15923E853E0C6F");

            entity.Property(e => e.Status).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__2A49584C8CA7142B");

            entity.Property(e => e.IdRol).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF975D98C2A5");

            entity.ToTable("Usuario");

            entity.Property(e => e.Clave)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK_Usuario_Roles");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.Status)
                .HasConstraintName("FK_Usuario_Estados");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
