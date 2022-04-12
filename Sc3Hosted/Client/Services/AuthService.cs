using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Sc3Hosted.Client.Helpers;
using Sc3Hosted.Shared.Dtos;
namespace Sc3Hosted.Client.Services;
public interface IAuthService
{
    Task<LoginResultDto> Login(LoginDto loginModel);
    Task<RegisterResultDto> Register(RegisterDto registerModel);
    Task Logout();
}

public class AuthService : IAuthService
{
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }

    public async Task<RegisterResultDto> Register(RegisterDto registerModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/accounts", registerModel);
        var result = await response.Content.ReadFromJsonAsync<RegisterResultDto>();
        return result??new RegisterResultDto();
    }

    public async Task<LoginResultDto> Login(LoginDto loginModel)
    {
        var response = await _httpClient.PostAsJsonAsync("api/login", loginModel);
        var result = await response.Content.ReadFromJsonAsync<LoginResultDto>()??new LoginResultDto();

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
