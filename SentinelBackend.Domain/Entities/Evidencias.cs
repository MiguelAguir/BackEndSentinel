// Evidencias.cs
namespace SentinelBackend.Domain.Entities;

public class Evidencias
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TareaId { get; set; }
    public string Tipo { get; set; } = null!; // foto | texto | archivo
    public string? Url { get; set; }
    public string? Texto { get; set; }
    public bool? Aprobado { get; set; }
    public string? ComentarioSupervisor { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime Created_At { get; set; } = DateTime.UtcNow;

    public Tareas Tarea { get; set; } = null!;
    public Usuarios Usuario { get; set; } = null!;
}