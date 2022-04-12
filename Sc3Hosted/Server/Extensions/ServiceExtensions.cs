using Sc3Hosted.Server.Middleware;
using Sc3Hosted.Server.Services;
namespace Sc3Hosted.Server.Extensions;
public static class ServiceExtensions
{
    public static void ServiceWrapper(this IServiceCollection services)
    {
        services.AddTransient<IAssetService, AssetService>();
        services.AddTransient<ICommunicateService, CommunicateService>();
        services.AddTransient<ISituationService, SituationService>();
        services.AddTransient<ILocationService, LocationService>();
        
        services.AddScoped<IUserContextService, UserContextService>();
        services.AddScoped<ErrorHandlingMiddleware>();
    }
}
