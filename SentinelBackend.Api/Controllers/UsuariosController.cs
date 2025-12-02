using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelBackend.Domain.Entities;
using SentinelBackend.Domain.Ports;
using System.Security.Claims;
using SentinelBackend.Application.DTOs;

namespace SentinelBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuariosController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public UsuariosController(IUnitOfWork uow) => _uow = uow;

    private string Rol => User.FindFirst(ClaimTypes.Role)?.Value!;
    private Guid UsuarioDbId
    {
        get
        {
            var claim = User.FindFirst("db_user_id")?.Value;
            return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
        }
    }

    // POST: Registrar nuevo subordinado (solo supervisor)
    [HttpPost("registrar-subordinado")]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> RegistrarSubordinado([FromBody] RegistrarUsuarioDto dto)
    {
        var nuevoUsuario = new Usuarios
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Nombre = dto.Nombre,
            Rol = "usuario",
            SupervisorId = UsuarioDbId,  // el que crea es su supervisor
            FirebaseUid = dto.FirebaseUid, // lo obtienes del frontend tras crear en Firebase Auth
            Created_At = DateTime.UtcNow
        };

        await _uow.Usuarios.AddAsync(nuevoUsuario);
        await _uow.SaveChangesAsync();

        return CreatedAtAction("GetUsuario", new { id = nuevoUsuario.Id }, nuevoUsuario);
    }

    // GET: Mis subordinados (solo supervisor)
    [HttpGet("mis-subordinados")]
    [Authorize(Roles = "supervisor")]
    public async Task<IActionResult> GetMisSubordinados()
    {
        var subordinados = await _uow.Usuarios.FindAsync(u => u.SupervisorId == UsuarioDbId);
        return Ok(subordinados);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUsuario(Guid id)
    {
        var usuario = await _uow.Usuarios.GetByIdAsync(id);
        if (usuario == null) return NotFound();
        return Ok(usuario);
    }
}

