// Services/AuthService.cs
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TestTechApp.Models;

namespace TestTechApp.Services;
public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(string username, string password);
}

public class AuthService : IAuthService
{
    private readonly HttpClient _http;
    private readonly ILogger<AuthService> _logger;

    public AuthService(HttpClient http, ILogger<AuthService> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(string username, string password)
    {
        try
        {
            _logger.LogInformation("Attempting login for user {Username}", username);
            var response = await _http.PostAsJsonAsync("https://dummyjson.com/auth/login",
                new LoginRequest { Username = username, Password = password });

            response.EnsureSuccessStatusCode();
            var login = await response.Content.ReadFromJsonAsync<LoginResponse>();
            _logger.LogInformation("Login succeeded for user {Username}", username);
            return login;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for user {Username}", username);
            return null;
        }
    }
}