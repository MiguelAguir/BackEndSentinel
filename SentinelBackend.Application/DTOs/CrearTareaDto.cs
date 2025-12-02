namespace SentinelBackend.Application.DTOs;
public record CrearTareaDto(
    string Titulo,
    string? Descripcion,
    DateTime? FechaLimite,
    int Puntos,
    bool EvidenciaRequerida,
    Guid UsuarioId);