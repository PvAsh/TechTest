using System.Text.Json;
using TestTechApp.Models;
using Xunit;
namespace TestTechApp.Tests;

public class LoginResponseTests
{
    [Fact]
    public void DeserializeLoginResponse_Works()
    {
        var json = @"{
            ""accessToken"": ""token123"",
            ""refreshToken"": ""refresh123"",
            ""id"": 1,
            ""username"": ""emilys""
        }";

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var login = JsonSerializer.Deserialize<LoginResponse>(json, options);

        Assert.NotNull(login);
        Assert.Equal("token123", login!.AccessToken);
        Assert.Equal("refresh123", login.RefreshToken);
        Assert.Equal("emilys", login.Username);
        Assert.Equal(1, login.Id);
    }
}