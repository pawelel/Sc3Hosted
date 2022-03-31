using Blazored.LocalStorage;
using Sc3Hosted.Client.Helpers;
using Sc3Hosted.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Sc3Hosted.Client.Services;

public interface IAuthService
{
    Task<LoginResult> Login(LoginModel loginModel);
    Task<RegisterResult> Register(RegisterModel registerModel);
    Task Logout();
}
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }

    public async Task<RegisterResult> Register(RegisterModel registerModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/accounts", registerModel);
        var result = await response.Content.ReadFromJsonAsync<RegisterResult>();
        return result??new RegisterResult();
    }

    public async Task<LoginResult> Login(LoginModel loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/login", loginModel);
        var result = await response.Content.ReadFromJsonAsync<LoginResult>()??new LoginResult();

        if (!result.Successful) return result;
        await _localStorage.SetItemAsync("authToken", result.Token);
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(result.Token);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Token);
        return result;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}