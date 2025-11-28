// Recompensas.cs
namespace SentinelBackend.Domain.Entities;

public class Recompensas
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int CostoPuntos { get; set; }
    public Guid? UsuarioId { get; set; }
    public Guid? SupervisorId { get; set; }
    public bool Canjeada { get; set; } = false;
    public DateTime Created_At { get; set; } = DateTime.UtcNow;

    public Usuarios? Usuario { get; set; }
    public Usuarios? Supervisor { get; set; }
}