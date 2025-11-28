using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelBackend.Domain.Entities;
using SentinelBackend.Domain.Ports;
using System.Security.Claims;

namespace SentinelBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TareasController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public TareasController(IUnitOfWork uow) => _uow = uow;

    private string FirebaseUid => User.FindFirst("user_id")?.Value!;
    private string Rol => User.FindFirst(ClaimTypes.Role)?.Value!;
    private Guid UsuarioDbId => Guid.Parse(User.FindFirst("db_user_id")?.Value!);

    // GET: Mis tareas
    [HttpGet("mis-tareas")]
    public async Task<IActionResult> GetMisTareas()
    {
        var tareas = await _uow.Tareas.FindAsync(t => t.UsuarioId == UsuarioDbId);
        return Ok(tareas);
    }

    // GET: Tareas de mis subordinados (solo supervisor)
    [HttpGet("subordinados")]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> GetTareasSubordinados()
    {
        var subordinados = await _uow.Usuarios.FindAsync(u => u.SupervisorId == UsuarioDbId);
        var ids = subordinados.Select(u => u.Id);

        var tareas = await _uow.Tareas.FindAsync(t => ids.Contains(t.UsuarioId));
        return Ok(tareas);
    }

    // GET: Tarea por ID (para CreatedAtAction)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTareaById(Guid id)
    {
        var tarea = await _uow.Tareas.GetByIdAsync(id);
        if (tarea == null) return NotFound();
        return Ok(tarea);
    }

    // POST: Crear tarea
    [HttpPost]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> Crear([FromBody] CrearTareaDto dto)
    {
        // Usuario normal solo puede crear para sí mismo
        if (Rol != "supervisor" && dto.UsuarioId != UsuarioDbId)
            return Forbid();

        var tarea = new Tareas
        {
            Id = Guid.NewGuid(),
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            FechaLimite = dto.FechaLimite,
            Puntos = dto.Puntos,
            EvidenciaRequerida = dto.EvidenciaRequerida,
            Estado = "pendiente",
            UsuarioId = dto.UsuarioId,
            SupervisorId = Rol == "supervisor" ? UsuarioDbId : null,
            Created_At = DateTime.UtcNow
        };

        await _uow.Tareas.AddAsync(tarea);
        await _uow.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTareaById), new { id = tarea.Id }, tarea);
    }

    // PUT: Actualizar tarea
    [HttpPut("{id}")]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> Actualizar(Guid id, [FromBody] ActualizarTareaDto dto)
    {
        var tarea = await _uow.Tareas.GetByIdAsync(id);
        if (tarea == null) return NotFound();

        if (Rol != "supervisor" && tarea.UsuarioId != UsuarioDbId)
            return Forbid();

        tarea.Titulo = dto.Titulo ?? tarea.Titulo;
        tarea.Descripcion = dto.Descripcion ?? tarea.Descripcion;
        tarea.FechaLimite = dto.FechaLimite ?? tarea.FechaLimite;
        tarea.Puntos = dto.Puntos ?? tarea.Puntos;

        _uow.Tareas.Update(tarea);
        await _uow.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: Eliminar tarea (solo supervisor o dueño)
    [HttpDelete("{id}")]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        var tarea = await _uow.Tareas.GetByIdAsync(id);
        if (tarea == null) return NotFound();

        if (Rol != "supervisor" && tarea.UsuarioId != UsuarioDbId)
            return Forbid();

        _uow.Tareas.Remove(tarea);
        await _uow.SaveChangesAsync();

        return NoContent();
    }
}

// DTOs (ponlos dentro del mismo archivo o en carpeta Dtos)
public record CrearTareaDto(
    string Titulo,
    string? Descripcion,
    DateTime? FechaLimite,
    int Puntos,
    bool EvidenciaRequerida,
    Guid UsuarioId);

public record ActualizarTareaDto(
    string? Titulo,
    string? Descripcion,
    DateTime? FechaLimite,
    int? Puntos);