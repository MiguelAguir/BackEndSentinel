// Logros.cs
namespace SentinelBackend.Domain.Entities;

public class Logros
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public Guid? TareaId { get; set; }
    public int Puntos { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    public Usuarios Usuario { get; set; } = null!;
    public Tareas? Tarea { get; set; }
}