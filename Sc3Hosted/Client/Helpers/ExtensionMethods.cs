using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using Microsoft.AspNetCore.Components;
namespace Sc3Hosted.Client.Helpers;
public static class ExtensionMethods
{
    public static NameValueCollection QueryString(this NavigationManager navigationManager)
    {
        return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
    }

    public static string QueryString(this NavigationManager navigationManager, string key)
    {
        return navigationManager.QueryString()[key]??string.Empty;
    }
    public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
        where TAttribute : Attribute
    {
        var result = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<TAttribute>();
        return result!;
    }
}
