// SentinelBackend.Infrastructure/Persistence/UnitOfWork.cs

using SentinelBackend.Domain.Entities;
using SentinelBackend.Domain.Ports;

namespace SentinelBackend.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IRepository<Usuarios> Usuarios { get; private set; }
    public IRepository<Metas> Metas { get; private set; }
    public IRepository<Tareas> Tareas { get; private set; }
    public IRepository<Evidencias> Evidencias { get; private set; }
    public IRepository<Recompensas> Recompensas { get; private set; }
    public IRepository<Logros> Logros { get; private set; }
    public IRepository<Gamificacion> Gamificacion { get; private set; }
    public IRepository<Perfiles> Perfiles { get; private set; }
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Usuarios = new Repository<Usuarios>(context);
        Metas = new Repository<Metas>(context);
        Tareas = new Repository<Tareas>(context);
        Evidencias = new Repository<Evidencias>(context);
        Recompensas = new Repository<Recompensas>(context);
        Logros = new Repository<Logros>(context);
        Gamificacion = new Repository<Gamificacion>(context);
        Perfiles = new Repository<Perfiles>(context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}