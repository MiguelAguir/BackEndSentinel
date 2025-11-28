namespace SentinelBackend.Domain.Entities;

public class Perfiles
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UsuarioId { get; set; }
    public string Atributos { get; set; } = "{}";

    public DateTime Created_At { get; set; } = DateTime.UtcNow;

    public Usuarios Usuario { get; set; } = null!;
}