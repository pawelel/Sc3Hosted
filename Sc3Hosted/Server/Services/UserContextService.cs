using System.Security.Claims;
namespace Sc3Hosted.Server.Services;
public interface IUserContextService
{
    ClaimsPrincipal User { get; }
    string GetUserId { get; }
    string GetCurrentUserName();
}

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    
    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User??new ClaimsPrincipal();
    public string GetUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value??string.Empty;

    public string GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.Name??string.Empty;
    }
}
