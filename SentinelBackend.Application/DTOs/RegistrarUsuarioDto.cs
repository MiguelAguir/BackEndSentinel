namespace SentinelBackend.Application.DTOs;

public record RegistrarUsuarioDto(
    string Email, 
    string Nombre, 
    string FirebaseUid
    );