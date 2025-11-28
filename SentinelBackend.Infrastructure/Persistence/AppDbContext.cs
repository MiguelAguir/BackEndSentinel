using Microsoft.EntityFrameworkCore;
using SentinelBackend.Domain.Entities;
using SentinelBackend.Domain.Ports;

namespace SentinelBackend.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Usuarios> Usuarios => Set<Usuarios>();
    public DbSet<Metas> Metas => Set<Metas>();
    public DbSet<Tareas> Tareas => Set<Tareas>();
    public DbSet<Evidencias> Evidencias => Set<Evidencias>();
    public DbSet<Recompensas> Recompensas => Set<Recompensas>();
    public DbSet<Gamificacion> Gamificacion => Set<Gamificacion>();
    public DbSet<Logros> Logros => Set<Logros>();
    public DbSet<Perfiles> Perfiles => Set<Perfiles>();

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // === TODAS LAS RELACIONES (que ya tenías) ===
    modelBuilder.Entity<Gamificacion>()
        .HasKey(g => g.UsuarioId);
    modelBuilder.Entity<Gamificacion>()
        .HasOne(g => g.Usuario)
        .WithOne()
        .HasForeignKey<Gamificacion>(g => g.UsuarioId);

    modelBuilder.Entity<Perfiles>()
        .HasOne(p => p.Usuario)
        .WithOne()
        .HasForeignKey<Perfiles>(p => p.UsuarioId);

    modelBuilder.Entity<Perfiles>()
        .Property(p => p.Atributos)
        .HasColumnType("jsonb");

    modelBuilder.Entity<Usuarios>()
        .HasOne(u => u.Supervisor)
        .WithMany(u => u.Subordinados)
        .HasForeignKey(u => u.SupervisorId)
        .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<Tareas>()
        .HasOne(t => t.Supervisor)
        .WithMany(u => u.TareasAsignadas)
        .HasForeignKey(t => t.SupervisorId);

    modelBuilder.Entity<Tareas>()
        .HasOne(t => t.Usuario)
        .WithMany(u => u.TareasPropias)
        .HasForeignKey(t => t.UsuarioId);

    modelBuilder.Entity<Metas>()
        .HasOne(m => m.Usuario)
        .WithMany(u => u.Metas)
        .HasForeignKey(m => m.UsuarioId);

    modelBuilder.Entity<Tareas>()
        .HasOne(t => t.Meta)
        .WithMany(m => m.Tareas)
        .HasForeignKey(t => t.MetaId)
        .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<Evidencias>()
        .HasOne(e => e.Tarea)
        .WithMany(t => t.Evidencias)
        .HasForeignKey(e => e.TareaId);

    modelBuilder.Entity<Evidencias>()
        .HasOne(e => e.Usuario)
        .WithMany()
        .HasForeignKey(e => e.UsuarioId);

    modelBuilder.Entity<Logros>()
        .HasOne(l => l.Usuario)
        .WithMany()
        .HasForeignKey(l => l.UsuarioId);

    modelBuilder.Entity<Logros>()
        .HasOne(l => l.Tarea)
        .WithMany()
        .HasForeignKey(l => l.TareaId)
        .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<Recompensas>()
        .HasOne(r => r.Usuario)
        .WithMany()
        .HasForeignKey(r => r.UsuarioId)
        .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<Recompensas>()
        .HasOne(r => r.Supervisor)
        .WithMany()
        .HasForeignKey(r => r.SupervisorId)
        .OnDelete(DeleteBehavior.SetNull);

    modelBuilder.Entity<Usuarios>().ToTable("usuarios");
    modelBuilder.Entity<Metas>().ToTable("metas");
    modelBuilder.Entity<Tareas>().ToTable("tareas");
    modelBuilder.Entity<Evidencias>().ToTable("evidencias");
    modelBuilder.Entity<Recompensas>().ToTable("recompensas");
    modelBuilder.Entity<Gamificacion>().ToTable("gamificacion");
    modelBuilder.Entity<Logros>().ToTable("logros");
    modelBuilder.Entity<Perfiles>().ToTable("perfiles");

    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
        foreach (var property in entity.GetProperties())
        {
            var propertyName = property.Name;
            var tempName = System.Text.RegularExpressions.Regex.Replace(propertyName, "_", "");
            var columnName = string.Concat(tempName.Select((x, i) => 
                i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLowerInvariant();
            columnName = columnName.Replace("__", "_");

            property.SetColumnName(columnName);
        }
    }

    base.OnModelCreating(modelBuilder);

    base.OnModelCreating(modelBuilder);
}
}
