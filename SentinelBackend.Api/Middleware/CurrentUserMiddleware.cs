// CurrentUserMiddleware.cs
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SentinelBackend.Infrastructure.Persistence;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var firebaseUid = context.User.FindFirst("user_id")?.Value;

            if (!string.IsNullOrEmpty(firebaseUid))
            {
                var usuario = await db.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

                if (usuario != null)
                {
                    // ESTOS SON LOS CLAIMS QUE ASP.NET LEE PARA [Authorize(Roles = "supervisor")]
                    var claims = new List<Claim>
                    {
                        new Claim("db_user_id", usuario.Id.ToString()),
                        new Claim(ClaimTypes.Role, usuario.Rol ?? "usuario"), // ← ESTE ES EL IMPORTANTE
                        new Claim(ClaimTypes.Name, usuario.Nombre ?? usuario.Email),
                        new Claim("role", usuario.Rol ?? "usuario")
                    };

                    // FORZAMOS QUE SE RECONOZCAN LOS ROLES
                    var identity = new ClaimsIdentity(claims, "Firebase");
                    var principal = new ClaimsPrincipal(identity);
                    
                    // REEMPLAZAMOS EL USER ACTUAL
                    context.User = principal;

                    context.Items["UsuarioActual"] = usuario;
                }
            }
        }

        await _next(context);
    }
}