using System.Text.Json;

namespace TestTechApp.Services;

public interface IDummyLoginService
{
    Task<(string Username, string Password)> GetRandomUserAsync();
}

public class DummyLoginService : IDummyLoginService
{
    private readonly HttpClient _http;

    public DummyLoginService(HttpClient http)
    {
        _http = http;
    }

    public async Task<(string Username, string Password)> GetRandomUserAsync()
    {
        var response = await _http.GetAsync("https://dummyjson.com/users");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var users = doc.RootElement.GetProperty("users");

        var creds = new List<(string Username, string Password)>();

        foreach (var user in users.EnumerateArray())
        {
            var username = user.GetProperty("username").GetString()!;
            var password = user.GetProperty("password").GetString()!;
            creds.Add((username, password));
        }

        var random = new Random();
        return creds[random.Next(creds.Count)];
    }
}
