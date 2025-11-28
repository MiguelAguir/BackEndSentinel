using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SentinelBackend.Infrastructure.Persistence;

namespace SentinelBackend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly AppDbContext _db;

    public TestController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("usuarios")]
    public async Task<IActionResult> GetUsuarios()
    {
        var usuarios = await _db.Usuarios
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.Nombre,
                u.Rol,
                u.Created_At
            })
            .ToListAsync();

        return Ok(new { total = usuarios.Count, usuarios });
    }
}