namespace SentinelBackend.Application.DTOs;

public record ActualizarTareaDto(

    string? Titulo,
    string? Descripcion,
    DateTime? FechaLimite,
    int? Puntos);