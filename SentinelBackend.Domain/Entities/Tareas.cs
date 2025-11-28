// Tareas.cs
namespace SentinelBackend.Domain.Entities;

public class Tareas
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = null!;
    public string? Descripcion { get; set; }
    public DateTime? FechaLimite { get; set; }
    public string Estado { get; set; } = "pendiente"; // pendiente | en_progreso | completada | aprobada
    public bool EvidenciaRequerida { get; set; } = true;
    public bool EvidenciaSubida { get; set; } = false;
    public double? PuntuacionIa { get; set; }
    public string? Recompensa { get; set; }
    public Guid? MetaId { get; set; }
    public Guid? SupervisorId { get; set; }
    public Guid UsuarioId { get; set; }
    public int Puntos { get; set; } = 100;
    public DateTime Created_At { get; set; } = DateTime.UtcNow;

    public Metas? Meta { get; set; }
    public Usuarios Supervisor { get; set; } = null!;
    public Usuarios Usuario { get; set; } = null!;
    public ICollection<Evidencias> Evidencias { get; set; } = new List<Evidencias>();
}