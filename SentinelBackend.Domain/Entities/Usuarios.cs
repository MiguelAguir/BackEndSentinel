// Usuarios.cs
using System.ComponentModel.DataAnnotations;

namespace SentinelBackend.Domain.Entities;

public class Usuarios
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirebaseUid { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Rol { get; set; } = "usuario";
    public Guid? SupervisorId { get; set; }
    public DateTime Created_At { get; set; } = DateTime.UtcNow;

    // Navegación
    public Usuarios? Supervisor { get; set; }
    public ICollection<Usuarios> Subordinados { get; set; } = new List<Usuarios>();
    public ICollection<Metas> Metas { get; set; } = new List<Metas>();
    public ICollection<Tareas> TareasAsignadas { get; set; } = new List<Tareas>();
    public ICollection<Tareas> TareasPropias { get; set; } = new List<Tareas>();
}