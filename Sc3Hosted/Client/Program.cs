using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Sc3Hosted.Client;
using Sc3Hosted.Client.Helpers;
using Sc3Hosted.Client.Services;
using Sc3Hosted.Shared.Helpers;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore(config => {
    config.AddPolicy(Policies.IsAdmin, Policies.IsAdminPolicy());
    config.AddPolicy(Policies.IsUser, Policies.IsUserPolicy());
    config.AddPolicy(Policies.IsManager, Policies.IsManagerPolicy());
});
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAssetsService, AssetsService>();
builder.Services.AddScoped<IAuthService, AuthService>();

await builder.Build().RunAsync();
