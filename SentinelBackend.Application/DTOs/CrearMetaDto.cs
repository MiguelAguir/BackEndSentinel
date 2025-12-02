namespace SentinelBackend.Application.DTOs;

public record CrearMetaDto(
    string Titulo,
    string? Descripcion,
    int PuntosObjetivo,
    DateTime FechaInicio,
    DateTime? FechaFin,
    Guid UsuarioId);