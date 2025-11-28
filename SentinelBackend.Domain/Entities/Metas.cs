// Metas.cs
namespace SentinelBackend.Domain.Entities;

public class Metas
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int PuntosObjetivo { get; set; } = 1000;
    public int PuntosActuales { get; set; } = 0;
    public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
    public DateTime? FechaFin { get; set; }
    public bool Activa { get; set; } = true;
    public DateTime Created_At { get; set; } = DateTime.UtcNow;

    public Usuarios Usuario { get; set; } = null!;
    public ICollection<Tareas> Tareas { get; set; } = new List<Tareas>();
}