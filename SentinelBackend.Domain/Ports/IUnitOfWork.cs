// SentinelBackend.Domain/Ports/IUnitOfWork.cs

using SentinelBackend.Domain.Entities;

namespace SentinelBackend.Domain.Ports;

public interface IUnitOfWork : IDisposable
{
    IRepository<Usuarios> Usuarios { get; }
    IRepository<Metas> Metas { get; }
    IRepository<Tareas> Tareas { get; }
    IRepository<Evidencias> Evidencias { get; }
    IRepository<Recompensas> Recompensas { get; }
    IRepository<Logros> Logros { get; }
    IRepository<Gamificacion> Gamificacion { get; }
    IRepository<Perfiles> Perfiles { get; }

    Task<int> SaveChangesAsync();
}