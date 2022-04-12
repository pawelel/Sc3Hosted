using System.Security.Claims;
namespace Sc3Hosted.Server.Services;
public interface IUserContextService
{
    ClaimsPrincipal User { get; }
    string UserId { get; }
}

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User??new ClaimsPrincipal();
    public string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value??string.Empty;
}
