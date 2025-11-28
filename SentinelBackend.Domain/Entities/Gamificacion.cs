// Gamificacion.cs
namespace SentinelBackend.Domain.Entities;

public class Gamificacion
{
    public Guid UsuarioId { get; set; }
    public int PuntosTotales { get; set; } = 0;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Usuarios Usuario { get; set; } = null!;
}